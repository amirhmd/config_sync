#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

echo "Stopping local dev environment..."
docker compose down