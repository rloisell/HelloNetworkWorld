#!/usr/bin/env bash
# scripts/dev-status.sh
# Ryan Loiselle — Developer / Architect
# GitHub Copilot — AI pair programmer / code generation
# April 2026
#
# Thin wrapper — delegates to ~/dev-tools/dev-ctl (consolidated dev manager).
# Usage: ./scripts/dev-status.sh
exec "$HOME/dev-tools/dev-ctl" status
# (lines below are superseded)
exit 0

PASS="✓"
FAIL="✗"

check() {
  local name="$1"
  local cmd="$2"
  if eval "$cmd" >/dev/null 2>&1; then
    printf "  %s  %-20s UP\n" "$PASS" "$name"
    return 0
  else
    printf "  %s  %-20s DOWN\n" "$FAIL" "$name"
    return 1
  fi
}

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "  HelloNetworkWorld — Dev Stack Status"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

ALL_OK=true

check "MariaDB (:3306)"   "mariadb --skip-ssl -e 'SELECT 1;'" || ALL_OK=false
check "API live (:5200)"  "curl -sf --max-time 3 http://localhost:5200/health/live" || ALL_OK=false
check "API ready (:5200)" "curl -sf --max-time 3 http://localhost:5200/health/ready" || ALL_OK=false
check "Frontend (:5175)"  "curl -sf --max-time 3 http://localhost:5175" || ALL_OK=false

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

if $ALL_OK; then
  echo "  All services healthy."
else
  echo "  One or more services are DOWN. Run: ./scripts/dev-start.sh"
fi
echo ""
