# Use the official .NET SDK image. It contains the .NET runtime and all the build/test tools.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test-runner

WORKDIR /src

# Copy the solution file and all project files first.
# This leverages Docker's layer caching. If only code files change later,
# Docker won't need to restore all the NuGet packages again, speeding up subsequent builds.
COPY *.sln .
COPY CardValidation.Core/*.csproj ./CardValidation.Core/
COPY CardValidation.Web/*.csproj ./CardValidation.Web/
COPY CardValidation.Core.UnitTests/*.csproj ./CardValidation.Core.UnitTests/
COPY CardValidation.Web.IntegrationTests/*.csproj ./CardValidation.Web.IntegrationTests/

# Restore all dependencies for the solution.
RUN dotnet restore

# Copy the rest of the source code into the image.
COPY . .

# Make the test runner script executable and set it as the entrypoint.
RUN chmod +x ./run-tests.sh
ENTRYPOINT ["./run-tests.sh"]
