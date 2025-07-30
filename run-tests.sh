#!/bin/sh
# This script ensures that if any command fails, the script will exit immediately.
set -e

# Define the coverage threshold required by the project's README.
COVERAGE_THRESHOLD=80

echo "--- Cleaning up old test results ---"
# The 'rm -rf' can fail with "Device or resource busy" on some systems (e.g., Docker on Windows).
# This approach is more robust: ensure the directory exists, then delete its contents.
mkdir -p /src/test-results && find /src/test-results -mindepth 1 -delete

echo "--- Running unit and integration tests and collecting coverage data ---"
# We explicitly target the unit test project to ensure we are only measuring its coverage.
dotnet test --nologo --collect:"XPlat Code Coverage" --results-directory /src/test-results

echo "--- Installing ReportGenerator tool ---"
dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.3.6

# Add the .NET tools directory to the PATH for the current script session
export PATH="$PATH:/root/.dotnet/tools"

echo "--- Generating HTML coverage report ---"
# This command finds all Cobertura files, merges them, and creates an HTML report.
# We filter to only include assemblies from our actual application, not the tests.
reportgenerator \
    -reports:"/src/test-results/**/coverage.cobertura.xml" \
    -targetdir:"/src/test-results/coverage-report" \
    -reporttypes:Html \
    -assemblyfilters:"+CardValidation.Core;+CardValidation.Web"

echo "--- Checking if code coverage meets the ${COVERAGE_THRESHOLD}% threshold ---"
# Generate a simple text summary to easily parse the coverage percentage.
reportgenerator \
    -reports:"/src/test-results/**/coverage.cobertura.xml" \
    -targetdir:"/src/test-results/coverage-summary" \
    -reporttypes:TextSummary \
    -assemblyfilters:"+CardValidation.Core;+CardValidation.Web"

# Extract the line coverage percentage and fail the build if it's below the threshold.
LINE_COVERAGE=$(grep -oP 'Line coverage: \K[0-9]+' /src/test-results/coverage-summary/Summary.txt)

echo "Current tests coverage is ${LINE_COVERAGE}%."

if [ "$LINE_COVERAGE" -lt "$COVERAGE_THRESHOLD" ]; then
    echo "Error: Code coverage is below the ${COVERAGE_THRESHOLD}% threshold."
    exit 1
else
    echo "Success: Code coverage meets the threshold."
    exit 0
fi
