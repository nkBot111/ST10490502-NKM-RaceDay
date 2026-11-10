#!/usr/bin/env bash
# Move original root-level mixed projects aside. The corrected source is in
# Raceday.API, RaceDay.Web and RaceDay.Tests. This does not delete old work.
set -euo pipefail
cd "$(dirname "$0")/.."
for item in Raceday.API/Raceday.API.csproj RaceDay.Web/RaceDay.Web.csproj RaceDay.Tests/RaceDay.Tests.csproj Raceday.API/Program.cs; do
  [[ -f "$item" ]] || { echo "ERROR: corrected file missing: $item"; exit 1; }
done
mkdir -p .legacy-nkm-backup
for item in Controllers DTOs Data Migrations Models Services Views wwwroot obj Properties \
  AuthControllerTests.cs RoleAndEnrollmentTests.cs TestSession.cs \
  Program.cs Raceday.API.csproj RaceDay.Web.csproj RaceDay.Tests.csproj \
  appsettings.json appsettings.Development.json \
  Raceday.API.csproj.user RaceDay.Web.csproj.user Raceday.API.slnLaunch.user; do
  if [[ -e "$item" ]]; then
    mv "$item" ".legacy-nkm-backup/$item"
    echo "Backed up old root file/folder: $item"
  fi
done
printf '\nOld root files moved into .legacy-nkm-backup (excluded from Git).\n'
printf 'You can now run: bash scripts/setup-codespaces.sh\n'
