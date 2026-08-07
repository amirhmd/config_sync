#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

docker run --rm --net=host \
  -v "$(cd ../db && pwd)":/db \
  arigaio/atlas:1.3.0 schema apply \
  --url "postgres://configsync:configsync@localhost:5432/configsync?sslmode=disable" \
  --to file:///db/schema.hcl \
  --dev-url "postgres://atlas:atlas@localhost:5433/atlas?sslmode=disable" \
  --dry-run