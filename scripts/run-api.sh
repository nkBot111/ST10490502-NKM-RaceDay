#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
[[ -f .env ]] || { echo 'Run bash scripts/setup-codespaces.sh first'; exit 1; }
set -a
# shellcheck disable=SC1091
source .env
set +a
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__RaceDayConnection="Server=127.0.0.1,14333;Database=RaceDayNKM;User ID=sa;Password=${MSSQL_SA_PASSWORD};TrustServerCertificate=True;MultipleActiveResultSets=true;Connect Timeout=5"
exec dotnet run --project Raceday.API/Raceday.API.csproj --no-launch-profile --urls http://0.0.0.0:5081
