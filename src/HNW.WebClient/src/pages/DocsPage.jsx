/**
 * DocsPage.jsx — Network Reference Panel
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * March 2026
 *
 * Network-focused reference panel: OpenShift SDN, Silver/Gold/Emerald cluster
 * networking, BC Gov DataClass and zone model, NetworkPolicy patterns, SDN guidance.
 * Links are fetched from the API (ReferenceLinks table), falling back to
 * hardcoded defaults if the API is unavailable.
 *
 * Implements: 002-documentation-hub (pivoted March 2026 — network reference only)
 * AI-assisted: category + static fallback update for network pivot;
 * reviewed and directed by Ryan Loiselle.
 */

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { getReferenceLinks } from "../api/networkTestsApi";

// ── Fallback static links (shown when API is unavailable) ──────────────────

/** @type {Record<string, Array<{title: string, url: string, description: string}>>} */
const STATIC_LINKS = {
  OpenShiftNetworking: [
    {
      title: "OpenShift SDN Overview",
      url: "https://docs.openshift.com/container-platform/4.14/networking/openshift_sdn/about-openshift-sdn.html",
      description: "How the OpenShift SDN plugin manages pod networking and inter-namespace isolation.",
    },
    {
      title: "Network Policy in OpenShift",
      url: "https://docs.openshift.com/container-platform/4.14/networking/network_policy/about-network-policy.html",
      description: "Overview of Kubernetes NetworkPolicy and how OpenShift enforces default-deny.",
    },
    {
      title: "Configuring egress NetworkPolicy",
      url: "https://docs.openshift.com/container-platform/4.14/networking/network_policy/creating-network-policy.html",
      description: "Step-by-step guide to creating NetworkPolicy objects for egress traffic.",
    },
    {
      title: "BC Gov Private Cloud — Network Policies",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/",
      description: "Platform-specific NetworkPolicy guidance for BC Gov OpenShift namespaces.",
    },
    {
      title: "OVN-Kubernetes Overview",
      url: "https://docs.openshift.com/container-platform/4.14/networking/ovn_kubernetes_network_provider/about-ovn-kubernetes.html",
      description: "OVN-Kubernetes network provider used in newer OpenShift clusters including Emerald.",
    },
  ],
  ClusterTiers: [
    {
      title: "BC Gov Private Cloud Clusters",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters",
      description: "Comparison of Silver, Gold, and Emerald cluster capabilities, SDN implementations, and networking differences.",
    },
    {
      title: "Silver Cluster Networking Notes",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#silver",
      description: "Silver-specific networking: OpenShift SDN, HAProxy router, ingress/egress defaults.",
    },
    {
      title: "Gold Cluster Networking Notes",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#gold",
      description: "Gold-specific networking: OVN-Kubernetes, default-deny stance, multi-zone topology.",
    },
    {
      title: "Emerald Cluster Networking Notes",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#emerald",
      description: "Emerald-specific networking: strict default-deny ingress AND egress, AVI load balancer, DataClass enforcement.",
    },
    {
      title: "AVI / NSX Advanced Load Balancer (Emerald)",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#avi-nsx-advanced-load-balancer",
      description: "AVI replaces HAProxy on Emerald. Routes must carry the correct AVI InfraSettings annotation.",
    },
  ],
  DataClassAndZones: [
    {
      title: "BC Gov DataClass Overview",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification",
      description: "How BC Government classifies data (Low / Medium / High) and what each class means for network access.",
    },
    {
      title: "Network Zone Model (Public / Private / Restricted)",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#network-zones",
      description: "The three network zones and which data classes are permitted in each.",
    },
    {
      title: "DataClass Labels on Pods and Routes",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#labels",
      description: "Required pod label DataClass: Medium and how it interacts with Emerald's AVI InfraSettings.",
    },
    {
      title: "Impact of DataClass on Egress (Emerald)",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#emerald-impact",
      description: "How DataClass labelling determines which egress network zones your pods can reach on Emerald.",
    },
    {
      title: "AVI InfraSettings — dataclass-medium vs dataclass-low",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/avi-infrasettings",
      description: "Only dataclass-medium has a registered VIP on Emerald. Never use dataclass-low.",
    },
    {
      title: "BC Gov Information Security Classification Framework (ISCF)",
      url: "https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security/information-security-classification",
      description: "Foundation policy defining Protected A / B / C information classes. The OpenShift DataClass label (Low / Medium / High) maps directly to ISCF levels — Low = public info, Medium = Protected A / lower Protected B, High = Protected B-C.",
    },
    {
      title: "BC Gov Network Security Zone Model — Zone A / B / C / DMZ",
      url: "https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security",
      description: "Traditional OCIO zone model (IMIT Standard 6.13): Zone A = Restricted High Security (Protected B-C), Zone B = High Security (Protected A), Zone C = Trusted Client (managed IDIR devices), DMZ = internet-facing. SDN Low/Medium/High classifications map to these historic zones respectively.",
    },
  ],
  NetworkPolicyPatterns: [
    {
      title: "Two-policy rule: ingress + egress",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#two-policy-rule",
      description: "Every network flow requires TWO policies: ingress on the receiver AND egress on the sender.",
    },
    {
      title: "DNS egress policy (UDP+TCP 53)",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#dns",
      description: "All pods need an explicit DNS egress policy (UDP 53 + TCP 53) on Emerald — not included by default.",
    },
    {
      title: "Allow egress to external IP / CIDR",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#egress-cidr",
      description: "How to write a CIDR-based egress policy to allow traffic to external systems or on-prem networks.",
    },
    {
      title: "Least-privilege NetworkPolicy",
      url: "https://kubernetes.io/docs/concepts/services-networking/network-policies/#the-networkpolicy-resource",
      description: "Kubernetes reference: only open the specific port and protocol needed — never wildcard egress.",
    },
    {
      title: "Common port reference (Oracle 1521, MSSQL 1433, PG 5432, MySQL 3306)",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#common-ports",
      description: "Standard database and service ports used when writing egress NetworkPolicy rules.",
    },
    {
      title: "Third Party Gateway (3PG) / ExtraNet — external partner connectivity",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/",
      description: "Connections to external government partners (other ministries, Crown corps, health authorities) must traverse the ExtraNet zone via a Third Party Gateway (3PG). Requires formal approval and a dedicated egress NetworkPolicy rule targeting the 3PG CIDR.",
    },
  ],
  SdnGuidance: [
    {
      title: "Default-deny ingress and egress on Emerald",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#default-deny",
      description: "Emerald enforces default-deny on BOTH ingress and egress. Every flow must be explicitly allowed.",
    },
    {
      title: "Calico vs OVN-Kubernetes",
      url: "https://docs.openshift.com/container-platform/4.14/networking/ovn_kubernetes_network_provider/migrate-from-openshift-sdn.html",
      description: "Guidance on SDN migration and the differences between OpenShift SDN, Calico, and OVN-Kubernetes.",
    },
    {
      title: "Troubleshooting NetworkPolicy",
      url: "https://docs.openshift.com/container-platform/4.14/networking/network_policy/viewing-network-policy.html",
      description: "How to view and inspect NetworkPolicy objects on a running cluster to diagnose connectivity failures.",
    },
    {
      title: "oc debug — testing connectivity from a pod",
      url: "https://docs.openshift.com/container-platform/4.14/support/troubleshooting/troubleshooting-network-issues.html",
      description: "Use oc debug to launch a temporary pod and test egress connectivity with curl, nc, or nslookup.",
    },
    {
      title: "SDN Security Classification — Low / Medium / High workload model",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification",
      description: "2022 BC Gov SDN model: Low = public info (DMZ-equivalent, internet accessible), Medium = Protected A (no direct internet), High = Protected B-C (internet blocked at guardrail). DataClass pod label must match workload classification.",
    },
    {
      title: "Medium/High workloads — internet egress via Forward Proxy only",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#egress-cidr",
      description: "Medium security workloads cannot reach the internet directly — must use the SSBC SDN Forward Proxy (HTTP/HTTPS only). High workloads require Ministry ISO (MISO) exemption. Direct internet egress from Medium/High is denied at the SDN guardrail.",
    },
    {
      title: "Zone adjacency rule — no zone hopping",
      url: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#network-zones",
      description: "BC Gov zone adjacency rule: communication is only permitted between adjacent zones. Traffic path: Internet → DMZ/Low → Medium → High. A session cannot be initiated directly from the internet into Medium or High zones.",
    },
  ],
};

const CATEGORY_ORDER = [
  "OpenShiftNetworking",
  "ClusterTiers",
  "DataClassAndZones",
  "NetworkPolicyPatterns",
  "SdnGuidance",
];

const CATEGORY_LABELS = {
  OpenShiftNetworking:   "OpenShift Networking",
  ClusterTiers:          "Silver / Gold / Emerald",
  DataClassAndZones:     "DataClass & Zones",
  NetworkPolicyPatterns: "NetworkPolicy Patterns",
  SdnGuidance:           "SDN Guidance",
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
        <h1 style={{ margin: 0, color: "#003366" }}>Network Reference</h1>
        <p style={{ color: "#6b6b6b", marginTop: 8 }}>
          OpenShift SDN, cluster tier networking, BC Gov DataClass and zone model,
          NetworkPolicy patterns, and egress guidance for configured network tests.
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

      {/* Environment context note */}
      <div className="hnw-card" style={{ marginTop: 32 }}>
        <h3 style={{ marginTop: 0 }}>Environment context</h3>
        <p>
          Some links (OpenShift console, ArgoCD app) adapt based on the current deployment
          environment. When deployed to OpenShift, the API returns environment-specific URLs (e.g.,
          <code> https://hnw-be808f-dev.apps.emerald.devops.gov.bc.ca</code>).
        </p>
        <p style={{ marginBottom: 0 }}>
          To add additional network reference links, use <code>POST /api/reference-links</code>
          or edit the <code>ReferenceLinks</code> table directly.
        </p>
      </div>
    </>
  );
}
