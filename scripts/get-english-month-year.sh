#!/usr/bin/env bash
set -euo pipefail

#
# Get the current month and year in English format for mandoc(7) man pages.
#
# Returns "Month 1, YYYY" (e.g., "January 1, 2025") regardless of system locale.
# Used for updating .Dd date tags in man pages consistently across environments.
# The day is always 1 (first of the month) for reproducibility.
#
# Usage:
#   ./scripts/get-english-month-year.sh
#
# Output:
#   Prints "Month 1, YYYY" to stdout (e.g., "January 1, 2025")
#
# Exit codes:
#   0 - Success
#   1 - Unexpected error
#

month_num=$(date '+%m')
year=$(date '+%Y')

case "$month_num" in
  01) month_name="January" ;;
  02) month_name="February" ;;
  03) month_name="March" ;;
  04) month_name="April" ;;
  05) month_name="May" ;;
  06) month_name="June" ;;
  07) month_name="July" ;;
  08) month_name="August" ;;
  09) month_name="September" ;;
  10) month_name="October" ;;
  11) month_name="November" ;;
  12) month_name="December" ;;
  *)
    echo "ERROR: Unexpected month number: $month_num" >&2
    exit 1
    ;;
esac

echo "${month_name} 1, ${year}"
