---
name: ci-cd-pipeline
description: Configures and troubleshoots GitHub Actions CI/CD pipelines for BC Government OpenShift projects following ISB EA Option 2 — five-workflow pattern, image tag strategy, yq GitOps updates, Trivy scanning, and pipeline failure triage. Use when creating workflows, diagnosing failures, setting up branch protection, or updating the OpenShift GitOps image tag after a successful build.
metadata:
  author: Ryan Loiselle
  version: "1.0"
compatibility: GitHub Actions. Emerald OpenShift platform — see ../bc-gov-emerald/SKILL.md. Containerfile standards — see ../containerfile-standards/SKILL.md.
---

# CI/CD Pipeline Agent

Manages GitHub Actions workflows for BC Government projects on the Emerald OpenShift platform.

For container registry and Containerfile standards, see
[`../containerfile-standards/SKILL.md`](../containerfile-standards/SKILL.md).

For AVI InfraSettings, DataClass labels, and NetworkPolicy, see
[`../bc-gov-emerald/SKILL.md`](../bc-gov-emerald/SKILL.md).

---

## Architecture: ISB EA Option 2

```
GitHub (source) ──build──> Artifactory (images) ──gitops──> OpenShift (runtime)
                        ArgoCD (GitOps) polls a gitops-be<id> repo and applies Helm
```

Environments: `dev` → `test` → `prod`
Promotion is controlled by updating the image tag in the GitOps Helm values YAML.

---

## Five-Workflow Pattern

| File | Trigger | Purpose |
|------|---------|---------|
| `pr-checks.yml` | `pull_request` | Lint, unit tests, Trivy scan |
| `build-dev.yml` | `push: branches: [develop]` | Build + push image → deploy dev |
| `build-test.yml` | `push: branches: [test]` | Build + push image → deploy test |
| `build-prod.yml` | `push: tags: v*` | Build + push image → deploy prod |
| `dependency-review.yml` | `pull_request` | OSSF dependency-review action |

---

## Image Tag Strategy

```
<registry>/<org>/<project>/<service>:<tag>

Tags:
  PR:    pr-<number>-<sha7>      e.g. pr-42-abc1234
  Dev:   dev-<sha7>-<run>        e.g. dev-abc1234-87
  Test:  test-<sha7>-<run>       e.g. test-abc1234-87
  Prod:  <semver>                e.g. 1.2.3
```

---

## Build Workflow Steps

```yaml
jobs:
  build:
    runs-on: ubuntu-24.04
    steps:
      - uses: actions/checkout@v4

      - name: Set image tag
        id: tag
        run: echo "tag=dev-${GITHUB_SHA::7}-${{ github.run_number }}" >> "$GITHUB_OUTPUT"

      - name: Login to Artifactory
        uses: docker/login-action@v3
        with:
          registry: ${{ secrets.ARTIFACTORY_URL }}
          username: ${{ secrets.ARTIFACTORY_SERVICE_ACCOUNT }}
          password: ${{ secrets.ARTIFACTORY_SERVICE_ACCOUNT_TOKEN }}

      - name: Build and push
        uses: docker/build-push-action@v5
        with:
          context: .
          file: src/<project>/Containerfile
          push: true
          tags: ${{ secrets.ARTIFACTORY_URL }}/<org>/<project>/<service>:${{ steps.tag.outputs.tag }}

      - name: Trivy image scan
        uses: aquasecurity/trivy-action@master
        with:
          image-ref: ${{ secrets.ARTIFACTORY_URL }}/<org>/<project>/<service>:${{ steps.tag.outputs.tag }}
          format: table
          exit-code: 1
          vuln-type: os,library
          severity: CRITICAL,HIGH
```

---

## GitOps Image Tag Update (yq)

After a successful build, update the GitOps Helm values file and push:

```yaml
      - name: Update GitOps image tag
        run: |
          git clone https://x-access-token:${{ secrets.GITOPS_TOKEN }}@github.com/<org>/gitops-be<id>.git gitops
          cd gitops
          yq -i '.image.tag = "${{ steps.tag.outputs.tag }}"' \
            environments/dev/<service>/values.yaml
          git config user.email "ci@<project>"
          git config user.name "GitHub Actions"
          git commit -am "ci: update <service> image tag to ${{ steps.tag.outputs.tag }}"
          git push
```

---

## Secrets Required

| Secret | Description |
|--------|-------------|
| `ARTIFACTORY_URL` | Registry base URL |
| `ARTIFACTORY_SERVICE_ACCOUNT` | Robot account name |
| `ARTIFACTORY_SERVICE_ACCOUNT_TOKEN` | Robot account token |
| `GITOPS_TOKEN` | PAT — `repo` + `workflow` scopes only |
| `OC_SERVER` | OpenShift cluster API URL (optional for direct oc deploy) |
| `OC_TOKEN` | Service account token for direct oc (optional) |

---

## Placeholder Substitution

When starting from the template, replace all `<placeholder>` values. Find them with:

```bash
grep -r "<" .github/workflows/ | grep -v ".git"
```

---

## paths: Filter Discovery (2026-02-28)

When workflows use `paths:` filters that don't match changed files, the workflow
never triggers. Cause: path in filter doesn't match actual directory structure.

```bash
# Find all paths filters in workflows
grep -r "paths:" .github/workflows/

# Verify the actual paths that exist
find . -name "Containerfile" -not -path "./.git/*"
```

Fix: align `paths:` filters with the real directory tree.

---

## Status Check Configuration

**Branch protection must list the EXACT workflow job name.**

```bash
# List all job names across all workflows
grep -h "^  [a-z].*:$" .github/workflows/*.yml | sed 's/://g' | sort -u
```

In GitHub: Settings → Branches → Require status checks → paste each job name.

---

## PR Copilot Review

In `.github/workflows/pr-checks.yml`:

```yaml
      - name: Copilot code review
        uses: github/copilot-code-review@main
        with:
          github-token: ${{ secrets.GITHUB_TOKEN }}
```

Only runs on pull requests — no `GITHUB_TOKEN` permissions needed beyond default.

---

## Trivy Scan

```yaml
      - name: Trivy scan
        uses: aquasecurity/trivy-action@master
        with:
          image-ref: '<image>:<tag>'
          format: table
          exit-code: 1
          vuln-type: os,library
          severity: CRITICAL,HIGH
```

CRITICAL/HIGH → pipeline fails. Patch OS packages first (update base image),
then review library versions.

---

## Failure Patterns

| Symptom | Cause | Fix |
|---------|-------|-----|
| Workflow never triggers | `paths:` filter mismatch | Fix paths or remove filter |
| Artifactory auth denied | Token not in secrets | Add/rotate secret |
| Trivy CRITICAL found | Stale base image | Update base image tag in Containerfile |
| yq update silently no-ops | Wrong YAML key path | `yq '.'` to inspect actual key path |
| GitOps push rejected | Token lacks `Contents: write` | Add to PAT or use fine-grained PAT |
| Status check not required | Job name mismatch in branch protection | Run grep; update protection rule |

---

## PIPELINE_KNOWLEDGE

> Append new CI/CD discoveries here.
> Format: `YYYY-MM-DD: <discovery>`

- 2026-02-27: [HelloNetworkWorld] Paths filter fix — workflow was targeting wrong directory. `grep -r "paths:" .github/workflows/` revealed mismatch. Removing or fixing paths filter resolved silent non-trigger.
- 2026-02-27: [HelloNetworkWorld] Status checks must use exact job ID strings — copy from workflow YAML, not display names.
