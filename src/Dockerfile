FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and project files first for better layer caching
COPY PaymentsAPI.sln ./
COPY src/Shared.Contracts/Shared.Contracts.csproj ./Shared.Contracts/
COPY src/PaymentsAPI/PaymentsAPI.Worker.csproj ./PaymentsAPI/
COPY src/PaymentsAPI.Infrastructure/PaymentsAPI.Infrastructure.csproj ./PaymentsAPI.Infrastructure/

# Restore dependencies
RUN dotnet restore "PaymentsAPI/PaymentsAPI.Worker.csproj"

# Copy remaining source code
COPY src/. .

# Build and publish
RUN dotnet publish "PaymentsAPI/PaymentsAPI.Worker.csproj" -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /out .
ENTRYPOINT ["dotnet", "PaymentsAPI.Worker.dll"]
