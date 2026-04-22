#!/usr/bin/env bash
# scripts/dev-start.sh
# Ryan Loiselle — Developer / Architect
# GitHub Copilot — AI pair programmer / code generation
# April 2026
#
# Thin wrapper — delegates to ~/dev-tools/dev-ctl (consolidated dev manager).
# To manage all projects: dev-ctl start
# To manage only HNW:     dev-ctl start hnw
#
# Usage: ./scripts/dev-start.sh
exec "$HOME/dev-tools/dev-ctl" start hnw
# (lines below are superseded)
exit 0

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PID_DIR="/tmp/hnw-dev"
LOG_DIR="/tmp/hnw-dev"
API_LOG="$LOG_DIR/api.log"
FRONTEND_LOG="$LOG_DIR/frontend.log"
API_PID_FILE="$PID_DIR/api.pid"
FRONTEND_PID_FILE="$PID_DIR/frontend.pid"

mkdir -p "$PID_DIR"

# ── Helpers ───────────────────────────────────────────────────────────────────

info()    { echo "[HNW] $*"; }
success() { echo "[HNW] ✓ $*"; }
warn()    { echo "[HNW] ⚠ $*"; }
error()   { echo "[HNW] ✗ $*" >&2; }

is_running() {
  local pid_file="$1"
  local port="$2"
  if [[ -f "$pid_file" ]]; then
    local pid
    pid=$(cat "$pid_file")
    if kill -0 "$pid" 2>/dev/null; then
      return 0
    fi
  fi
  # Fallback: check port
  lsof -iTCP:"$port" -sTCP:LISTEN -n >/dev/null 2>&1
}

wait_for_port() {
  local port="$1"
  local name="$2"
  local max_wait=30
  local waited=0
  while ! lsof -iTCP:"$port" -sTCP:LISTEN -n >/dev/null 2>&1; do
    if [[ $waited -ge $max_wait ]]; then
      error "$name did not start within ${max_wait}s"
      return 1
    fi
    sleep 1
    (( waited++ ))
  done
  success "$name is listening on :$port"
}

# ── MariaDB ───────────────────────────────────────────────────────────────────

info "Checking MariaDB..."
if mariadb --skip-ssl -e "SELECT 1;" >/dev/null 2>&1; then
  success "MariaDB is already running"
else
  info "Starting MariaDB via brew services..."
  brew services start mariadb@10.11
  sleep 3
  if mariadb --skip-ssl -e "SELECT 1;" >/dev/null 2>&1; then
    success "MariaDB started"
  else
    error "MariaDB failed to start. Check: brew services list"
    exit 1
  fi
fi

# Ensure hnw_dev database exists
mariadb --skip-ssl -e "CREATE DATABASE IF NOT EXISTS hnw_dev;" 2>/dev/null
success "hnw_dev database ready"

# ── API (.NET 10) ─────────────────────────────────────────────────────────────

info "Checking API on :5200..."
if is_running "$API_PID_FILE" 5200; then
  success "API is already running"
else
  info "Starting HNW API..."
  cd "$REPO_ROOT/src/HNW.Api"
  nohup dotnet run --launch-profile HNW.Api \
    < /dev/null \
    > "$API_LOG" 2>&1 &
  API_PID=$!
  echo "$API_PID" > "$API_PID_FILE"
  cd "$REPO_ROOT"

  info "Waiting for API to start (PID $API_PID)..."
  wait_for_port 5200 "API"
fi

# ── Frontend (Vite) ───────────────────────────────────────────────────────────

info "Checking Frontend on :5175..."
if is_running "$FRONTEND_PID_FILE" 5175; then
  success "Frontend is already running"
else
  info "Starting Vite dev server..."
  # Kill any suspended vite processes that might hold the port
  pkill -9 -f "node.*vite" 2>/dev/null || true
  sleep 1

  cd "$REPO_ROOT/src/HNW.WebClient"
  nohup npx vite --mode development --port 5175 \
    < /dev/null \
    > "$FRONTEND_LOG" 2>&1 &
  FRONTEND_PID=$!
  echo "$FRONTEND_PID" > "$FRONTEND_PID_FILE"
  cd "$REPO_ROOT"

  info "Waiting for Vite to start (PID $FRONTEND_PID)..."
  wait_for_port 5175 "Frontend"
fi

# ── Summary ───────────────────────────────────────────────────────────────────

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "  HelloNetworkWorld — Local Dev Stack"
echo "  API:       http://localhost:5200"
echo "  Swagger:   http://localhost:5200/swagger"
echo "  Frontend:  http://localhost:5175"
echo "  Health:    http://localhost:5200/health/live"
echo ""
echo "  Logs:  $LOG_DIR/"
echo "  PIDs:  $PID_DIR/"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
