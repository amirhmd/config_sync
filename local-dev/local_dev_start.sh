#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

echo "Starting local dev environment..."
docker compose up -d

echo "Waiting for Postgres to be ready..."
until docker exec configsync-postgres pg_isready -U configsync > /dev/null 2>&1; do
  sleep 2
done

echo "Waiting for Atlas dev database to be ready..."
until docker exec configsync-atlas-dev-db pg_isready -U atlas > /dev/null 2>&1; do
  sleep 2
done

echo "Planning schema changes..."
./db_plan.sh

echo "Applying schema changes..."
./db_apply.sh

echo "Happy Coding!"