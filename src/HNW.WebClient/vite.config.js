/*
 * vite.config.js — HNW.WebClient
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Vite configuration: React plugin, API proxy for local dev,
 * port 5175 to match CORS allowed origins in appsettings.Development.json.
 * AI-assisted: proxy configuration; reviewed and directed by Ryan Loiselle.
 */

import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5175,
    proxy: {
      // Proxy all /api calls to the .NET API in development
      '/api': {
        target: 'http://localhost:5200',
        changeOrigin: true,
      },
      '/health': {
        target: 'http://localhost:5200',
        changeOrigin: true,
      },
    },
  },
  build: {
    outDir: 'dist',
    sourcemap: false,
    // esnext required — main.jsx uses top-level await to load /config.json
    // at runtime before React mounts (runtime config pattern for multi-env images)
    target: 'esnext',
  },
});
