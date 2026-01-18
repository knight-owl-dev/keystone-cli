#!/usr/bin/env bash
set -euo pipefail

# Returns the current month and year in English format: "Month YYYY"
# This script is locale-independent and works on both macOS (BSD) and Linux (GNU).

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

echo "${month_name} ${year}"
