/*
 * DashboardPage.jsx — HNW.WebClient
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Landing page dashboard. Shows summary cards, per-test status tiles,
 * trendline charts (on tile click), and a state changes timeline.
 * Auto-refreshes every 60 seconds via TanStack Query refetchInterval.
 * AI-assisted: dashboard layout scaffold; reviewed and directed by Ryan Loiselle.
 */

import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { getNetworkTests, getTestResultSummary } from '../api/networkTestsApi.js';

// ── BC GOV COLOUR PALETTE ────────────────────────────────────────────────────
const COLOUR_PASS    = '#2E8540'; // BC Gov green
const COLOUR_FAIL    = '#D8292F'; // BC Gov red
const COLOUR_PENDING = '#F2A900'; // BC Gov amber
const COLOUR_NAVY    = '#003366'; // BC Gov navy

// ── PAGE ─────────────────────────────────────────────────────────────────────
export default function DashboardPage() {
  const refreshMs = (window.__env__?.refreshIntervalSeconds ?? 60) * 1000;
  const [selectedTestId, setSelectedTestId] = useState(null);

  const { data: tests = [], isLoading } = useQuery({
    queryKey: ['network-tests'],
    queryFn: getNetworkTests,
    refetchInterval: refreshMs,
  });

  if (isLoading) return <LoadingSpinner />;

  const passing = tests.filter(t => t.latestResult?.isSuccess === true).length;
  const failing = tests.filter(t => t.latestResult?.isSuccess === false).length;
  const pending = tests.filter(t => !t.latestResult).length;

  return (
    <div className="hnw-dashboard">
      <h1>Network Health Dashboard</h1>

      {/* ── SUMMARY CARDS ────────────────────────────────────────── */}
      <div className="hnw-summary-cards">
        <SummaryCard label="Total Tests"  value={tests.length}  />
        <SummaryCard label="Passing"      value={passing}       colour={COLOUR_PASS}    />
        <SummaryCard label="Failing"      value={failing}       colour={COLOUR_FAIL}    />
        <SummaryCard label="Pending"      value={pending}       colour={COLOUR_PENDING} />
      </div>

      {/* ── TEST TILES ───────────────────────────────────────────── */}
      <h2>Tests</h2>
      {tests.length === 0 ? (
        <EmptyState
          message="No network tests configured."
          action="Go to Network Tests to add your first test."
          actionHref="/tests"
        />
      ) : (
        <div className="hnw-test-tiles">
          {tests.map(test => (
            <TestTile
              key={test.id}
              test={test}
              isSelected={test.id === selectedTestId}
              onClick={() => setSelectedTestId(test.id === selectedTestId ? null : test.id)}
            />
          ))}
        </div>
      )}

      {/* ── TRENDLINE CHART (expanded on tile click) ─────────────── */}
      {selectedTestId && <TrendlineChart testId={selectedTestId} />}
    </div>
  );
} // end DashboardPage

// ── SUBCOMPONENTS ────────────────────────────────────────────────────────────

function SummaryCard({ label, value, colour = COLOUR_NAVY }) {
  return (
    <div className="hnw-summary-card" style={{ borderTopColor: colour }}>
      <div className="hnw-summary-card__value" style={{ color: colour }}>{value}</div>
      <div className="hnw-summary-card__label">{label}</div>
    </div>
  );
} // end SummaryCard

function TestTile({ test, isSelected, onClick }) {
  const result  = test.latestResult;
  const status  = !result ? 'pending' : result.isSuccess ? 'pass' : 'fail';
  const colour  = status === 'pass' ? COLOUR_PASS : status === 'fail' ? COLOUR_FAIL : COLOUR_PENDING;
  const statusLabel = status === 'pass' ? '✅ Healthy' : status === 'fail' ? '❌ Unhealthy' : '⏳ Pending';

  return (
    <button
      className={`hnw-test-tile ${isSelected ? 'hnw-test-tile--selected' : ''}`}
      style={{ borderLeftColor: colour }}
      onClick={onClick}
      aria-expanded={isSelected}
    >
      <div className="hnw-test-tile__header">
        <span className="hnw-test-tile__name">{test.name}</span>
        <span className="hnw-test-tile__status" style={{ color: colour }}>{statusLabel}</span>
      </div>
      <div className="hnw-test-tile__meta">
        <span>{test.destination}:{test.port ?? '—'}</span>
        <span className="hnw-test-tile__type">{test.serviceType}</span>
      </div>
      {result && (
        <div className="hnw-test-tile__last-check">
          Last: {new Date(result.executedAt).toLocaleString()}
          {result.latencyMs != null && <span> · {result.latencyMs}ms</span>}
          {result.errorMessage && <span className="hnw-test-tile__error"> · {result.errorMessage}</span>}
        </div>
      )}
      {test.policyStatus === 'PrPending' && (
        <div className="hnw-test-tile__policy-badge">
          🔔 Network Policy PR Pending —{' '}
          <a href={test.policyPrUrl} target="_blank" rel="noreferrer">View PR</a>
        </div>
      )}
    </button>
  );
} // end TestTile

function TrendlineChart({ testId }) {
  const { data: summary } = useQuery({
    queryKey: ['test-summary', testId, '7d'],
    queryFn: () => getTestResultSummary(testId, '7d'),
  });

  if (!summary) return null;

  return (
    <div className="hnw-trendline">
      <h3>7-Day Trend</h3>
      <div className="hnw-trendline__stats">
        <span>Uptime: <strong>{summary.uptimePercent?.toFixed(1)}%</strong></span>
        <span>Avg Latency: <strong>{summary.avgLatencyMs}ms</strong></span>
        <span>Runs: <strong>{summary.totalRuns}</strong></span>
      </div>
      <ResponsiveContainer width="100%" height={200}>
        <LineChart data={summary.dataPoints ?? []}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="time" tick={{ fontSize: 11 }} />
          <YAxis />
          <Tooltip />
          <Line type="monotone" dataKey="latencyMs" stroke={COLOUR_NAVY} dot={false} />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
} // end TrendlineChart

function EmptyState({ message, action, actionHref }) {
  return (
    <div className="hnw-empty-state">
      <p>{message}</p>
      {actionHref && <a href={actionHref} className="hnw-btn">{action}</a>}
    </div>
  );
} // end EmptyState

function LoadingSpinner() {
  return <div className="hnw-loading" aria-busy="true" aria-label="Loading…">Loading…</div>;
} // end LoadingSpinner
