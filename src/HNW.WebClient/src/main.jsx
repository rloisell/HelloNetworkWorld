/*
 * main.jsx — HNW.WebClient
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * React 18 entry point. Loads runtime config from /config.json,
 * sets up TanStack Query client, and renders the root App component.
 * AI-assisted: bootstrap scaffolding; reviewed and directed by Ryan Loiselle.
 */

import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import App from './App.jsx';
import './index.css';

// ── RUNTIME CONFIG ──────────────────────────────────────────────────────────
// /config.json is served by Nginx at runtime — never bake API URL at build time
const config = await fetch('/config.json').then(r => r.json()).catch(() => ({
  apiUrl: 'http://localhost:5200',
  refreshIntervalSeconds: 60,
}));

window.__env__ = config;

// ── TANSTACK QUERY ──────────────────────────────────────────────────────────
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: 2,
    },
  },
});

// ── RENDER ──────────────────────────────────────────────────────────────────
createRoot(document.getElementById('root')).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <App />
    </QueryClientProvider>
  </StrictMode>
);
