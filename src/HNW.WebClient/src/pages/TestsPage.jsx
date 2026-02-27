/**
 * TestsPage.jsx — Network Test Configuration
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Allows users to configure, enable/disable, and manually trigger network tests.
 * Includes cron expression builder, service type dropdown with default port,
 * and per-test result history modal.
 *
 * Implements: 003-network-test-config, 004-network-test-execution
 * AI-assisted: component scaffold + TanStack Query integration;
 * reviewed and directed by Ryan Loiselle.
 */

import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  getNetworkTests,
  createNetworkTest,
  updateNetworkTest,
  deleteNetworkTest,
  toggleNetworkTest,
  runNetworkTestNow,
  getTestResults,
} from "../api/networkTestsApi";

// ── Constants ──────────────────────────────────────────────────────────────

/** Service types with default port suggestions */
const SERVICE_TYPES = [
  { value: "TcpPort",         label: "TCP Port",           defaultPort: null },
  { value: "HttpEndpoint",    label: "HTTP Endpoint",      defaultPort: 443  },
  { value: "DnsResolve",      label: "DNS Resolve",        defaultPort: 53   },
  { value: "NtpServer",       label: "NTP Server",         defaultPort: 123  },
  { value: "SmtpRelay",       label: "SMTP Relay",         defaultPort: 25   },
  { value: "LdapServer",      label: "LDAP Server",        defaultPort: 389  },
  { value: "OidcProvider",    label: "OIDC Provider",      defaultPort: 443  },
  { value: "DatabaseServer",  label: "External Database",  defaultPort: null, hint: "Oracle 1521 · SQL Server 1433 · PostgreSQL 5432 · MySQL 3306" },
  { value: "FileService",     label: "File Service",       defaultPort: 445  },
  { value: "KubernetesApi",   label: "Kubernetes API",     defaultPort: 6443 },
  { value: "CustomTcp",       label: "Custom TCP",         defaultPort: null },
];

/** Common cron presets for the builder */
const CRON_PRESETS = [
  { label: "Every 5 minutes",  value: "0 */5 * * * ?" },
  { label: "Every 15 minutes", value: "0 */15 * * * ?" },
  { label: "Every 30 minutes", value: "0 */30 * * * ?" },
  { label: "Every hour",       value: "0 0 * * * ?" },
  { label: "Every 6 hours",    value: "0 0 */6 * * ?" },
  { label: "Daily at midnight",value: "0 0 0 * * ?" },
  { label: "Custom",           value: "custom" },
];

const EMPTY_FORM = {
  name:           "",
  host:           "",
  port:           "",
  serviceType:    "TcpPort",
  cronExpression: "0 */5 * * * ?",
  timeoutMs:      5000,
  isEnabled:      true,
  description:    "",
};

// ── Sub-components ──────────────────────────────────────────────────────────

function CronBuilder({ value, onChange }) {
  const [preset, setPreset] = useState(() => {
    const match = CRON_PRESETS.find((p) => p.value === value);
    return match ? match.value : "custom";
  });

  const handlePreset = (e) => {
    const selected = e.target.value;
    setPreset(selected);
    if (selected !== "custom") onChange(selected);
  };

  return (
    <div>
      <div className="hnw-form-group">
        <label htmlFor="cron-preset">Schedule preset</label>
        <select id="cron-preset" value={preset} onChange={handlePreset}>
          {CRON_PRESETS.map((p) => (
            <option key={p.value} value={p.value}>{p.label}</option>
          ))}
        </select>
      </div>
      <div className="hnw-form-group">
        <label htmlFor="cron-expression">Cron expression (6-field Quartz)</label>
        <input
          id="cron-expression"
          type="text"
          value={value}
          onChange={(e) => { setPreset("custom"); onChange(e.target.value); }}
          placeholder="0 */5 * * * ?"
        />
        <p className="hnw-form-hint">
          Format: <code>seconds minutes hours day-of-month month day-of-week</code>.
          Use <code>?</code> for unused day fields.
        </p>
      </div>
    </div>
  );
}

function TestFormModal({ test, onClose, onSaved }) {
  const isEdit = Boolean(test?.id);
  const [form, setForm] = useState(() =>
    isEdit
      ? {
          name:           test.name,
          host:           test.host,
          port:           test.port,
          serviceType:    test.serviceType,
          cronExpression: test.cronExpression,
          timeoutMs:      test.timeoutMs,
          isEnabled:      test.isEnabled,
          description:    test.description ?? "",
        }
      : { ...EMPTY_FORM }
  );
  const [errors, setErrors] = useState({});

  const queryClient = useQueryClient();
  const mutation = useMutation({
    mutationFn: isEdit
      ? (data) => updateNetworkTest(test.id, data)
      : createNetworkTest,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["network-tests"] });
      onSaved?.();
      onClose();
    },
  });

  const setField = (field, value) =>
    setForm((prev) => ({ ...prev, [field]: value }));

  const handleServiceTypeChange = (e) => {
    const svcType = e.target.value;
    const def = SERVICE_TYPES.find((s) => s.value === svcType);
    setForm((prev) => ({
      ...prev,
      serviceType: svcType,
      port: def?.defaultPort ?? (svcType === "DatabaseServer" ? "" : prev.port),
    }));
  };

  // Show hint for service types that require user-supplied port
  const activeServiceType = SERVICE_TYPES.find((s) => s.value === form.serviceType);
  const portHint = activeServiceType?.hint ?? null;

  const validate = () => {
    const errs = {};
    if (!form.name.trim())           errs.name = "Name is required";
    if (!form.host.trim())           errs.host = "Host / IP is required";
    if (!form.port || form.port < 1 || form.port > 65535)
      errs.port = "Port must be 1–65535";
    if (!form.cronExpression.trim()) errs.cronExpression = "Cron expression is required";
    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!validate()) return;
    mutation.mutate({ ...form, port: Number(form.port), timeoutMs: Number(form.timeoutMs) });
  };

  return (
    <div className="hnw-modal-backdrop" role="dialog" aria-modal="true" aria-labelledby="modal-title">
      <div className="hnw-modal">
        <div className="hnw-modal__header">
          <h2 id="modal-title">{isEdit ? "Edit Network Test" : "Add Network Test"}</h2>
          <button onClick={onClose} aria-label="Close">&times;</button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="hnw-modal__body">
            <div className="hnw-form-group">
              <label htmlFor="test-name">Name *</label>
              <input
                id="test-name"
                value={form.name}
                onChange={(e) => setField("name", e.target.value)}
                placeholder="e.g. DIAM OIDC endpoint"
              />
              {errors.name && <p className="hnw-form-error">{errors.name}</p>}
            </div>

            <div className="hnw-form-group">
              <label htmlFor="test-service-type">Service type *</label>
              <select id="test-service-type" value={form.serviceType} onChange={handleServiceTypeChange}>
                {SERVICE_TYPES.map((s) => (
                  <option key={s.value} value={s.value}>{s.label}</option>
                ))}
              </select>
            </div>

            <div style={{ display: "grid", gridTemplateColumns: "2fr 1fr", gap: "16px" }}>
              <div className="hnw-form-group" style={{ margin: 0 }}>
                <label htmlFor="test-host">Host / IP / FQDN *</label>
                <input
                  id="test-host"
                  value={form.host}
                  onChange={(e) => setField("host", e.target.value)}
                  placeholder="e.g. common-sso.justice.gov.bc.ca"
                />
                {errors.host && <p className="hnw-form-error">{errors.host}</p>}
              </div>
              <div className="hnw-form-group" style={{ margin: 0 }}>
                <label htmlFor="test-port">Port *</label>
                <input
                  id="test-port"
                  type="number"
                  min="1"
                  max="65535"
                  value={form.port}
                  onChange={(e) => setField("port", e.target.value)}
                  placeholder={portHint ? "Select port for your DB" : ""}
                />
                {portHint && <p className="hnw-form-hint">{portHint}</p>}
                {errors.port && <p className="hnw-form-error">{errors.port}</p>}
              </div>
            </div>

            <CronBuilder
              value={form.cronExpression}
              onChange={(v) => setField("cronExpression", v)}
            />
            {errors.cronExpression && (
              <p className="hnw-form-error">{errors.cronExpression}</p>
            )}

            <div className="hnw-form-group">
              <label htmlFor="test-timeout">Timeout (ms)</label>
              <input
                id="test-timeout"
                type="number"
                min="500"
                max="30000"
                value={form.timeoutMs}
                onChange={(e) => setField("timeoutMs", e.target.value)}
              />
              <p className="hnw-form-hint">500–30000 ms. Default: 5000.</p>
            </div>

            <div className="hnw-form-group">
              <label htmlFor="test-description">Description (optional)</label>
              <textarea
                id="test-description"
                rows={2}
                value={form.description}
                onChange={(e) => setField("description", e.target.value)}
                placeholder="What is this test checking?"
              />
            </div>

            <div className="hnw-form-group" style={{ marginBottom: 0 }}>
              <label style={{ display: "flex", alignItems: "center", gap: 8, fontWeight: "normal" }}>
                <input
                  type="checkbox"
                  checked={form.isEnabled}
                  onChange={(e) => setField("isEnabled", e.target.checked)}
                />
                Enable test immediately
              </label>
            </div>
          </div>

          <div className="hnw-modal__footer">
            <button type="button" className="btn-secondary" onClick={onClose}>Cancel</button>
            <button type="submit" className="btn-primary" disabled={mutation.isPending}>
              {mutation.isPending ? "Saving…" : isEdit ? "Save Changes" : "Add Test"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

function HistoryModal({ test, onClose }) {
  const { data: results = [], isLoading } = useQuery({
    queryKey: ["test-results", test.id],
    queryFn: () => getTestResults(test.id, 50),
  });

  return (
    <div className="hnw-modal-backdrop" role="dialog" aria-modal="true" aria-labelledby="history-title">
      <div className="hnw-modal" style={{ maxWidth: "800px" }}>
        <div className="hnw-modal__header">
          <h2 id="history-title">History — {test.name}</h2>
          <button onClick={onClose} aria-label="Close">&times;</button>
        </div>
        <div className="hnw-modal__body">
          {isLoading ? (
            <p className="hnw-loading">Loading…</p>
          ) : results.length === 0 ? (
            <p className="text-muted">No results yet.</p>
          ) : (
            <table className="hnw-table">
              <thead>
                <tr>
                  <th>Time</th>
                  <th>Result</th>
                  <th>Latency (ms)</th>
                  <th>Error</th>
                </tr>
              </thead>
              <tbody>
                {results.map((r) => (
                  <tr key={r.id}>
                    <td>{new Date(r.executedAt).toLocaleString()}</td>
                    <td>
                      <span className={`hnw-badge hnw-badge--${r.passed ? "pass" : "fail"}`}>
                        {r.passed ? "Pass" : "Fail"}
                      </span>
                    </td>
                    <td>{r.latencyMs ?? "—"}</td>
                    <td style={{ fontSize: "0.8125rem", color: "#6b6b6b" }}>{r.errorMessage ?? ""}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
        <div className="hnw-modal__footer">
          <button className="btn-secondary" onClick={onClose}>Close</button>
        </div>
      </div>
    </div>
  );
}

// ── Main page ───────────────────────────────────────────────────────────────

export default function TestsPage() {
  const queryClient = useQueryClient();
  const [showForm, setShowForm]       = useState(false);
  const [editTest, setEditTest]       = useState(null);
  const [historyTest, setHistoryTest] = useState(null);

  const { data: tests = [], isLoading, isError } = useQuery({
    queryKey: ["network-tests"],
    queryFn: getNetworkTests,
    refetchInterval: 30_000,
  });

  const toggleMutation = useMutation({
    mutationFn: ({ id, enabled }) => toggleNetworkTest(id, enabled),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["network-tests"] }),
  });

  const deleteMutation = useMutation({
    mutationFn: deleteNetworkTest,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["network-tests"] }),
  });

  const runNowMutation = useMutation({
    mutationFn: runNetworkTestNow,
    onSuccess: () => {
      setTimeout(() => queryClient.invalidateQueries({ queryKey: ["network-tests"] }), 3000);
    },
  });

  const handleDelete = (test) => {
    if (!window.confirm(`Delete "${test.name}"? This will remove all history.`)) return;
    deleteMutation.mutate(test.id);
  };

  const svcLabel = (type) =>
    SERVICE_TYPES.find((s) => s.value === type)?.label ?? type;

  return (
    <>
      <div className="flex justify-between items-center" style={{ marginBottom: 24 }}>
        <h1 style={{ margin: 0, color: "#003366" }}>Network Tests</h1>
        <button className="btn-primary" onClick={() => { setEditTest(null); setShowForm(true); }}>
          + Add Test
        </button>
      </div>

      {isLoading && <p className="hnw-loading">Loading tests…</p>}
      {isError && (
        <div className="hnw-alert hnw-alert--error">
          Failed to load tests. Is the API running?
        </div>
      )}

      {!isLoading && tests.length === 0 && (
        <div className="hnw-alert hnw-alert--info">
          No tests configured yet. Click <strong>+ Add Test</strong> to create your first network reachability test.
        </div>
      )}

      {tests.length > 0 && (
        <table className="hnw-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Host : Port</th>
              <th>Type</th>
              <th>Schedule</th>
              <th>Status</th>
              <th>Last Result</th>
              <th>Network Policy</th>
              <th style={{ textAlign: "right" }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {tests.map((t) => (
              <tr key={t.id} style={{ opacity: t.isEnabled ? 1 : 0.6 }}>
                <td>
                  <strong>{t.name}</strong>
                  {t.description && (
                    <div style={{ fontSize: "0.8125rem", color: "#6b6b6b" }}>{t.description}</div>
                  )}
                </td>
                <td style={{ fontFamily: "monospace" }}>
                  {t.host}:{t.port}
                </td>
                <td>{svcLabel(t.serviceType)}</td>
                <td style={{ fontSize: "0.8125rem", fontFamily: "monospace" }}>{t.cronExpression}</td>
                <td>
                  <span className={`hnw-badge hnw-badge--${t.isEnabled ? (t.lastResult?.passed ? "pass" : "fail") : "disabled"}`}>
                    {t.isEnabled ? (t.lastResult ? (t.lastResult.passed ? "Pass" : "Fail") : "Pending") : "Disabled"}
                  </span>
                </td>
                <td style={{ fontSize: "0.8125rem" }}>
                  {t.lastResult
                    ? `${t.lastResult.latencyMs ?? "—"} ms — ${new Date(t.lastResult.executedAt).toLocaleTimeString()}`
                    : "—"}
                </td>
                <td>
                  {t.policyStatus === "PrPending" && t.policyPrUrl ? (
                    <a
                      href={t.policyPrUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="hnw-badge hnw-badge--pending"
                      title="Network policy PR open — awaiting merge"
                      style={{ textDecoration: "none" }}
                    >
                      PR Open
                    </a>
                  ) : (
                    <span className="hnw-badge hnw-badge--disabled" style={{ opacity: 0.5 }}>
                      {t.policyStatus ?? "Unknown"}
                    </span>
                  )}
                </td>
                <td>
                  <div className="flex flex-gap" style={{ justifyContent: "flex-end" }}>
                    <button
                      className="btn-secondary"
                      style={{ padding: "4px 10px", fontSize: "0.8125rem" }}
                      onClick={() => runNowMutation.mutate(t.id)}
                      disabled={!t.isEnabled || runNowMutation.isPending}
                      title="Run test now"
                    >
                      ▶ Run
                    </button>
                    <button
                      className="btn-secondary"
                      style={{ padding: "4px 10px", fontSize: "0.8125rem" }}
                      onClick={() => setHistoryTest(t)}
                    >
                      History
                    </button>
                    <button
                      className="btn-secondary"
                      style={{ padding: "4px 10px", fontSize: "0.8125rem" }}
                      onClick={() => { setEditTest(t); setShowForm(true); }}
                    >
                      Edit
                    </button>
                    <button
                      className="btn-secondary"
                      style={{ padding: "4px 10px", fontSize: "0.8125rem" }}
                      onClick={() => toggleMutation.mutate({ id: t.id, enabled: !t.isEnabled })}
                    >
                      {t.isEnabled ? "Disable" : "Enable"}
                    </button>
                    <button
                      className="btn-danger"
                      style={{ padding: "4px 10px", fontSize: "0.8125rem" }}
                      onClick={() => handleDelete(t)}
                    >
                      Delete
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {showForm && (
        <TestFormModal
          test={editTest}
          onClose={() => { setShowForm(false); setEditTest(null); }}
        />
      )}

      {historyTest && (
        <HistoryModal
          test={historyTest}
          onClose={() => setHistoryTest(null)}
        />
      )}
    </>
  );
}
