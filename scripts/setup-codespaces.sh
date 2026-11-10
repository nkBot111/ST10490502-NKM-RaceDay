#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
command -v dotnet >/dev/null || { echo 'ERROR: .NET 8 SDK is not available'; exit 1; }
command -v docker >/dev/null || { echo 'ERROR: Docker is not installed in this Codespace'; exit 1; }
docker info >/dev/null || { echo 'ERROR: Docker daemon is not running'; exit 1; }
if [[ ! -f .env ]]; then
  command -v openssl >/dev/null || { echo 'ERROR: openssl is not installed'; exit 1; }
  printf 'MSSQL_SA_PASSWORD=RDay_%saA9!\n' "$(openssl rand -hex 14)" > .env
  chmod 600 .env
fi
set -a
# shellcheck disable=SC1091
source .env
set +a
docker compose up -d sqlserver
echo 'Building the 3-project solution...'
dotnet restore Raceday.API.sln
dotnet build Raceday.API.sln --no-restore
echo 'Running xUnit tests...'
dotnet test Raceday.API.sln --no-build
printf '\nSetup complete. Open two new terminals:\n'
printf '  bash scripts/run-api.sh\n  bash scripts/run-web.sh\n'
printf 'Then test with: python3 scripts/smoke-test.py\n'
