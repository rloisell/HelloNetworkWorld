#!/usr/bin/env bash
# scripts/dev-install-agent.sh
# Ryan Loiselle — Developer / Architect
# GitHub Copilot — AI pair programmer / code generation
# April 2026
#
# Thin wrapper — delegates to ~/dev-tools/dev-ctl (consolidated dev manager).
# Usage:
#   ./scripts/dev-install-agent.sh           # install monitor cron job
#   ./scripts/dev-install-agent.sh uninstall # remove monitor cron job
if [[ "${1:-}" == "uninstall" ]]; then
  exec "$HOME/dev-tools/dev-ctl" uninstall-monitor
else
  exec "$HOME/dev-tools/dev-ctl" install-monitor
fi
# (lines below are superseded)
exit 0

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
MONITOR_SCRIPT="$REPO_ROOT/scripts/dev-monitor.sh"
CRON_MARKER="# hnw-dev-monitor"
CRON_LINE="*/5 * * * * /bin/bash \"$MONITOR_SCRIPT\" $CRON_MARKER"

if [[ "${1:-}" == "uninstall" ]]; then
  # Remove the HNW cron entry
  { crontab -l 2>/dev/null | grep -v "$CRON_MARKER" || true; } | crontab -
  echo "[HNW] Monitor cron job removed."
  exit 0
fi

# ── Add cron entry (idempotent) ───────────────────────────────────────────────

# Remove any existing hnw-dev-monitor entry first, then add fresh
# (crontab -l exits 1 if empty; grep -v exits 1 if no other lines — both OK here)
{
  crontab -l 2>/dev/null | grep -v "$CRON_MARKER" || true
  echo "$CRON_LINE"
} | crontab -

echo "[HNW] Monitor cron job installed."
echo "[HNW] Schedule: every 5 minutes"
echo "[HNW] Script:   $MONITOR_SCRIPT"
echo "[HNW] Log:      /tmp/hnw-dev/monitor.log"
echo "[HNW] Alert:    macOS notification when a service is down"
echo ""
echo "[HNW] Current crontab:"
crontab -l 2>/dev/null | grep "$CRON_MARKER" || echo "(none found)"
echo ""
echo "[HNW] To uninstall: ./scripts/dev-install-agent.sh uninstall"
