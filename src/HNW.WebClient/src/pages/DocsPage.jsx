/**
 * DocsPage.jsx — BC Gov Standards Documentation Hub
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Provides a tabbed reference hub of all BC Government DevOps, Design,
 * Security, and Deployment standards, with dynamic cluster-relative links.
 * Links are fetched from the API (ReferenceLinks table), falling back to
 * hardcoded defaults if the API is unavailable.
 *
 * Implements: 002-documentation-hub
 * AI-assisted: component scaffold + tab navigation;
 * reviewed and directed by Ryan Loiselle.
 */

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { getReferenceLinks } from "../api/networkTestsApi";

// ── Fallback static links (shown when API is unavailable) ──────────────────

/** @type {Record<string, Array<{title: string, url: string, description: string}>>} */
const STATIC_LINKS = {
  Design: [
    {
      title: "BC Gov Design System",
      url: "https://design.gov.bc.ca",
      description: "Official BC Government design tokens, components, and style guides.",
    },
    {
      title: "Design Tokens npm (@bcgov/design-tokens)",
      url: "https://www.npmjs.com/package/@bcgov/design-tokens",
      description: "CSS custom properties and design tokens for React/web projects.",
    },
    {
      title: "BC Gov Font (BCSans)",
      url: "https://fonts2.gov.bc.ca",
      description: "BCSans web font for official BC Gov applications.",
    },
  ],
  Development: [
    {
      title: "DevOps Platform Services Docs",
      url: "https://docs.developer.gov.bc.ca",
      description: "Comprehensive guide to building and deploying on OpenShift Emerald.",
    },
    {
      title: "BC Gov GitHub Org (bcgov-c)",
      url: "https://github.com/bcgov-c",
      description: "Internal (private) GitHub organization for BC Gov projects.",
    },
    {
      title: "Artifactory (dbe8-docker-local)",
      url: "https://artifacts.developer.gov.bc.ca",
      description: "Container image registry for be808f namespace. Push images here.",
    },
    {
      title: "OpenShift Emerald Console",
      url: "https://console.apps.emerald.devops.gov.bc.ca",
      description: "OpenShift cluster console for Emerald (requires VPN/IDIR).",
    },
  ],
  Security: [
    {
      title: "IMIT Cyber Security Policy",
      url: "https://www2.gov.bc.ca/gov/content/governments/services-for-government/policies-procedures/cyber-security/cyber-security-policy",
      description: "BC Government cyber security policy — DataClass and handling requirements.",
    },
    {
      title: "Information Security Classification",
      url: "https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security/information-security-classification",
      description: "How to classify data (Protected A/B/C, etc.) for BC Gov systems.",
    },
    {
      title: "Common SSO (Keycloak)",
      url: "https://common-sso.justice.gov.bc.ca",
      description: "Justice common hosted Keycloak OIDC. Used for Phase 2 auth in HNW.",
    },
    {
      title: "GETOK — Service Client Registration",
      url: "https://getok.nrs.gov.bc.ca",
      description: "Register a Keycloak service client for machine-to-machine auth.",
    },
  ],
  OpenShift: [
    {
      title: "Deploy to OpenShift",
      url: "https://docs.developer.gov.bc.ca/deploy-to-openshift/",
      description: "DevOps Platform guide to deploying workloads on OpenShift.",
    },
    {
      title: "OpenShift NetworkPolicy",
      url: "https://docs.developer.gov.bc.ca/openshift-network-policies/",
      description: "Configuring NetworkPolicies for namespaces on OpenShift.",
    },
    {
      title: "Resource Tuning",
      url: "https://docs.developer.gov.bc.ca/openshift-resource-tuning/",
      description: "Tuning CPU/memory requests and limits for OpenShift pods.",
    },
    {
      title: "Emerald AVI / HAProxy",
      url: "https://docs.developer.gov.bc.ca/openshift-routes/",
      description: "Route and AVI InfraSettings on Emerald. Always use dataclass-medium.",
    },
  ],
  GitOps: [
    {
      title: "ArgoCD (Platform)",
      url: "https://argocd.developer.gov.bc.ca",
      description: "GitOps continuous delivery — syncs Helm charts from tenant-gitops-be808f.",
    },
    {
      title: "tenant-gitops-be808f (GitHub)",
      url: "https://github.com/bcgov-c/tenant-gitops-be808f",
      description: "Shared GitOps repo for the be808f namespace. HNW Helm chart lives here.",
    },
    {
      title: "Helm Docs",
      url: "https://helm.sh/docs/",
      description: "Official Helm 3 documentation for chart authoring.",
    },
  ],
  AIGuidance: [
    {
      title: "GitHub Copilot Docs",
      url: "https://docs.github.com/en/copilot",
      description: "GitHub Copilot code assistant documentation.",
    },
    {
      title: "HNW Copilot Instructions",
      url: "https://github.com/bcgov-c/HelloNetworkWorld/blob/main/.github/copilot-instructions.md",
      description: "Project-specific AI guardrails and domain rules for HNW development.",
    },
    {
      title: "Spec-Kitty (Spec-driven development)",
      url: "https://github.com/bcgov-c/HelloNetworkWorld/tree/main/kitty-specs",
      description: "Feature spec files, plans, and WP task tracking for HNW.",
    },
  ],
  LocalEnvironment: [
    {
      title: "Local Development Guide",
      url: "https://github.com/bcgov-c/HelloNetworkWorld/blob/main/docs/local-development/README.md",
      description: "How to run the API (port 5200), frontend (port 5175), and MariaDB locally.",
    },
    {
      title: "Podman Desktop",
      url: "https://podman-desktop.io",
      description: "Container management tool used instead of Docker on BC Gov developer machines.",
    },
    {
      title: "EF Core Migrations",
      url: "https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/",
      description: "Entity Framework Core migration docs — used for HNW database schema management.",
    },
  ],
};

const CATEGORY_ORDER = [
  "Design",
  "Development",
  "Security",
  "OpenShift",
  "GitOps",
  "AIGuidance",
  "LocalEnvironment",
];

const CATEGORY_LABELS = {
  Design:           "Design",
  Development:      "Development",
  Security:         "Security",
  OpenShift:        "OpenShift",
  GitOps:           "GitOps / ArgoCD",
  AIGuidance:       "AI Guidance",
  LocalEnvironment: "Local Dev",
};

// ── Main page ───────────────────────────────────────────────────────────────

export default function DocsPage() {
  const [activeTab, setActiveTab] = useState(CATEGORY_ORDER[0]);

  const { data: apiLinks, isError: apiError } = useQuery({
    queryKey: ["reference-links"],
    queryFn: getReferenceLinks,
    retry: 1,
    staleTime: 5 * 60 * 1000, // 5 min — docs don't change often
  });

  /** Build the link list for the active category, preferring API data */
  const getLinks = (category) => {
    if (apiLinks) {
      const filtered = apiLinks.filter((l) => l.category === category);
      if (filtered.length > 0) return filtered;
    }
    return STATIC_LINKS[category] ?? [];
  };

  const links = getLinks(activeTab);

  return (
    <>
      <div style={{ marginBottom: 24 }}>
        <h1 style={{ margin: 0, color: "#003366" }}>BC Gov Standards Hub</h1>
        <p style={{ color: "#6b6b6b", marginTop: 8 }}>
          Authoritative reference links for design, development, security, and deployment standards
          used in BC Government projects.
        </p>
      </div>

      {apiError && (
        <div className="hnw-alert hnw-alert--warning" style={{ marginBottom: 16 }}>
          Could not reach the API — showing default links. Custom links you have saved may not appear.
        </div>
      )}

      {/* Category tabs */}
      <div className="hnw-docs-tabs" role="tablist">
        {CATEGORY_ORDER.map((cat) => (
          <button
            key={cat}
            role="tab"
            aria-selected={activeTab === cat}
            className={`hnw-docs-tab${activeTab === cat ? " hnw-docs-tab--active" : ""}`}
            onClick={() => setActiveTab(cat)}
          >
            {CATEGORY_LABELS[cat]}
          </button>
        ))}
      </div>

      {/* Links grid */}
      <div role="tabpanel" aria-label={CATEGORY_LABELS[activeTab]}>
        {links.length === 0 ? (
          <p className="text-muted">No links defined for this category yet.</p>
        ) : (
          <div className="hnw-docs-links">
            {links.map((link) => (
              <a
                key={link.url}
                href={link.url}
                target="_blank"
                rel="noopener noreferrer"
                className="hnw-docs-link-card"
              >
                <div className="hnw-docs-link-card__title">{link.title}</div>
                {link.description && (
                  <div className="hnw-docs-link-card__desc">{link.description}</div>
                )}
                <div style={{ fontSize: "0.75rem", color: "#6b6b6b", marginTop: 6, wordBreak: "break-all" }}>
                  {link.url}
                </div>
              </a>
            ))}
          </div>
        )}
      </div>

      {/* Cluster-relative links note */}
      <div className="hnw-card" style={{ marginTop: 32 }}>
        <h3 style={{ marginTop: 0 }}>Cluster-relative links</h3>
        <p>
          Some links (OpenShift console, ArgoCD, route URLs) adapt based on the current deployment
          environment. When deployed to OpenShift, the API returns environment-specific URLs (e.g.,
          <code> https://hnw-be808f-dev.apps.emerald.devops.gov.bc.ca</code>).
        </p>
        <p style={{ marginBottom: 0 }}>
          To add or customise links, use the <code>ReferenceLinks</code> database table or the API
          endpoint <code>POST /api/reference-links</code>.
        </p>
      </div>
    </>
  );
}
