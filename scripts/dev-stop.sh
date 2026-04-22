#!/usr/bin/env bash
# scripts/dev-stop.sh
# Ryan Loiselle — Developer / Architect
# GitHub Copilot — AI pair programmer / code generation
# April 2026
#
# Thin wrapper — delegates to ~/dev-tools/dev-ctl (consolidated dev manager).
# Usage: ./scripts/dev-stop.sh
exec "$HOME/dev-tools/dev-ctl" stop hnw
# (lines below are superseded)
exit 0

set -euo pipefail

PID_DIR="/tmp/hnw-dev"
API_PID_FILE="$PID_DIR/api.pid"
FRONTEND_PID_FILE="$PID_DIR/frontend.pid"
STOP_ALL=false

if [[ "${1:-}" == "--all" ]]; then
  STOP_ALL=true
fi

info()    { echo "[HNW] $*"; }
success() { echo "[HNW] ✓ $*"; }

# ── Frontend ──────────────────────────────────────────────────────────────────

info "Stopping Vite frontend..."
pkill -f "node.*vite" 2>/dev/null && success "Vite stopped" || info "Vite was not running"
rm -f "$FRONTEND_PID_FILE"

# ── API ───────────────────────────────────────────────────────────────────────

info "Stopping HNW API..."
if [[ -f "$API_PID_FILE" ]]; then
  API_PID=$(cat "$API_PID_FILE")
  # Kill the dotnet process tree
  pkill -P "$API_PID" 2>/dev/null || true
  kill "$API_PID" 2>/dev/null && success "API stopped (PID $API_PID)" || info "API was not running"
  rm -f "$API_PID_FILE"
else
  pkill -f "dotnet.*HNW.Api" 2>/dev/null && success "API stopped" || info "API was not running"
fi

# ── MariaDB (optional) ────────────────────────────────────────────────────────

if $STOP_ALL; then
  info "Stopping MariaDB..."
  brew services stop mariadb@10.11 && success "MariaDB stopped"
else
  info "MariaDB left running (pass --all to stop it too)"
fi

echo ""
echo "[HNW] All HNW dev services stopped."
