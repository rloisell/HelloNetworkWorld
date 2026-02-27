# Plan — Feature 007: Network Policy Automation

## Phase 1 — Policy evaluation service
- `INetworkPolicyService` and `NetworkPolicyService`
- Fetch current `networkpolicies.yaml` from `tenant-gitops-be808f` via GitHub API
- Parse existing egress rules to check if destination+port is already covered
- `NetworkPolicyCheckResult`: `Covered`, `NotCovered`

## Phase 2 — PR creation
- If `NotCovered`: generate `NetworkPolicy` YAML for the new egress rule
- GitHub API calls:
  1. Create branch `feat/hnw-egress-{slug}-{port}`
  2. Commit new policy YAML (append to networkpolicies.yaml as additional `---` block)
  3. Open PR via GitHub REST API
- Store PR URL + `policyStatus = PrPending` on the `NetworkTestDefinition`

## Phase 3 — API hooks
- Trigger policy check async after `POST /api/network-tests`
- `GET /api/network-tests/{id}` includes `policyStatus` and `policyPrUrl`
- `POST /api/network-tests/{id}/refresh-policy` — manually re-trigger policy check

## Phase 4 — React UI
- Test tile on dashboard shows policy status badge (✅ Covered, 🔔 PR Pending, ⚠️ Unknown)
- Link to PR URL when status is PR Pending

## Phase 5 — Configuration
- `GITHUB_TOKEN` env var (Vault → Kubernetes Secret)
- Skip automation gracefully when token not present
