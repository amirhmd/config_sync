#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

docker run --rm --net=host -it \
  arigaio/atlas:1.2.3 schema clean \
  --url "postgres://configsync:configsync@localhost:5432/configsync?sslmode=disable"