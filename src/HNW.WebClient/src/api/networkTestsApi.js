/*
 * networkTestsApi.js — HNW.WebClient
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Axios service functions for the /api/network-tests endpoints.
 * All API calls centralised here — never inline fetch/axios in components.
 * AI-assisted: service function scaffolding; reviewed and directed by Ryan Loiselle.
 */

import axios from 'axios';
import { getAuthHeaders } from './AuthConfig.js';

// ── CLIENT ───────────────────────────────────────────────────────────────────
const client = axios.create({
  baseURL: window.__env__?.apiUrl ?? 'http://localhost:5200',
  timeout: 15_000,
});

// adds Authorization header when a valid token is available
client.interceptors.request.use(config => {
  const headers = getAuthHeaders();
  if (headers.Authorization) config.headers.Authorization = headers.Authorization;
  return config;
});

// ── QUERIES ──────────────────────────────────────────────────────────────────

// returns all network test definitions with latest result summary
export const getNetworkTests = async () => {
  const { data } = await client.get('/api/network-tests');
  return data;
};

// returns a single test definition by id
export const getNetworkTest = async (id) => {
  const { data } = await client.get(`/api/network-tests/${id}`);
  return data;
};

// returns aggregated result summary for a test (window: '24h' | '7d' | '30d')
export const getTestResultSummary = async (id, window = '7d') => {
  const { data } = await client.get(`/api/network-tests/${id}/results/summary`, {
    params: { window },
  });
  return data;
};

// returns paginated result history for a test
export const getTestResults = async (id, page = 1, pageSize = 50) => {
  const { data } = await client.get(`/api/network-tests/${id}/results`, {
    params: { page, pageSize },
  });
  return data;
};

// ── MUTATIONS ────────────────────────────────────────────────────────────────

// creates a new network test definition
export const createNetworkTest = async (payload) => {
  const { data } = await client.post('/api/network-tests', payload);
  return data;
};

// updates an existing network test definition
export const updateNetworkTest = async (id, payload) => {
  const { data } = await client.put(`/api/network-tests/${id}`, payload);
  return data;
};

// deletes a network test definition
export const deleteNetworkTest = async (id) => {
  await client.delete(`/api/network-tests/${id}`);
};

// toggles enable/disable state of a test
export const toggleNetworkTest = async (id) => {
  const { data } = await client.patch(`/api/network-tests/${id}/toggle`);
  return data;
};

// triggers an immediate probe run outside of schedule
export const runNetworkTestNow = async (id) => {
  const { data } = await client.post(`/api/network-tests/${id}/run-now`);
  return data;
};

// ── REFERENCE LINKS (documentation hub) ─────────────────────────────────────

// returns all reference links (category + URL + description)
export const getReferenceLinks = async () => {
  const { data } = await client.get('/api/reference-links');
  return data;
};
