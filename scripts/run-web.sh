#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
export ASPNETCORE_ENVIRONMENT=Development
export RaceDayApi__BaseUrl=http://localhost:5081/
exec dotnet run --project RaceDay.Web/RaceDay.Web.csproj --no-launch-profile --urls http://0.0.0.0:5080
