#!/usr/bin/env bash
# scripts/dev-monitor.sh
# Ryan Loiselle — Developer / Architect
# GitHub Copilot — AI pair programmer / code generation
# April 2026
#
# Thin wrapper — delegates to ~/dev-tools/dev-ctl (consolidated dev manager).
# The crontab entry now points directly to dev-ctl monitor.
# Usage: ./scripts/dev-monitor.sh
exec "$HOME/dev-tools/dev-ctl" monitor

set -uo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
LOG_FILE="/tmp/hnw-dev/monitor.log"
STATE_FILE="/tmp/hnw-dev/monitor.state"

mkdir -p "$(dirname "$LOG_FILE")"

# ── Helpers ───────────────────────────────────────────────────────────────────

log() {
  echo "$(date '+%Y-%m-%d %H:%M:%S') $*" >> "$LOG_FILE"
}

notify() {
  local title="$1"
  local message="$2"
  # macOS notification via osascript
  osascript -e "display notification \"$message\" with title \"$title\" sound name \"Basso\"" 2>/dev/null || true
  # Also log it
  log "ALERT: $title — $message"
}

check_service() {
  local name="$1"
  local cmd="$2"
  if eval "$cmd" >/dev/null 2>&1; then
    return 0
  else
    return 1
  fi
}

# ── Load previous state ───────────────────────────────────────────────────────

prev_state="all_ok"
if [[ -f "$STATE_FILE" ]]; then
  prev_state=$(cat "$STATE_FILE")
fi

# ── Health checks ─────────────────────────────────────────────────────────────

FAILED=()

check_service "MariaDB"   "mariadb --skip-ssl -e 'SELECT 1;'"                         || FAILED+=("MariaDB (:3306)")
check_service "API-live"  "curl -sf --max-time 3 http://localhost:5200/health/live"   || FAILED+=("API /health/live (:5200)")
check_service "API-ready" "curl -sf --max-time 3 http://localhost:5200/health/ready"  || FAILED+=("API /health/ready (:5200)")
check_service "Frontend"  "curl -sf --max-time 3 http://localhost:5175"               || FAILED+=("Frontend (:5175)")

# ── Evaluate and alert ────────────────────────────────────────────────────────

if [[ ${#FAILED[@]} -eq 0 ]]; then
  # All services are healthy
  if [[ "$prev_state" != "all_ok" ]]; then
    # Was degraded, now recovered
    notify "HNW Dev Stack — Recovered" "All services are healthy again"
    log "RECOVERED: all services healthy"
  else
    log "OK: all services healthy"
  fi
  echo "all_ok" > "$STATE_FILE"
else
  # Some services are down
  failed_list=$(printf '%s, ' "${FAILED[@]}" | sed 's/, $//')
  notify "HNW Dev Stack — DOWN" "Down: $failed_list"
  echo "degraded" > "$STATE_FILE"

  # Suggest remediation
  log "DEGRADED: $failed_list"
  log "Tip: run $REPO_ROOT/scripts/dev-start.sh to restart"
fi
