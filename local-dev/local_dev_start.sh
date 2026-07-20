#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

echo "Starting local dev environment..."
docker compose up -d

echo "Happy Coding!"