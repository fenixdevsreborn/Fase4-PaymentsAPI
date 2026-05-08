# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os arquivos de projeto (Tudo com "Ms" minúsculo agora!)
COPY ["Ms.Payments.Worker/Ms.Payments.Worker.csproj", "Ms.Payments.Worker/"]
COPY ["Ms.Payments.Application/Ms.Payments.Application.csproj", "Ms.Payments.Application/"]
COPY ["Ms.Payments.Domain/Ms.Payments.Domain.csproj", "Ms.Payments.Domain/"]
COPY ["Ms.Payments.Infrastructure/Ms.Payments.Infrastructure.csproj", "Ms.Payments.Infrastructure/"]

# Restaura dependências
RUN dotnet restore "Ms.Payments.Worker/Ms.Payments.Worker.csproj"

# Copia todo o código e faz o build
COPY . .
WORKDIR "/src/Ms.Payments.Worker"
RUN dotnet build "Ms.Payments.Worker.csproj" -c Release -o /app/build

# Estágio de Publish
FROM build AS publish
RUN dotnet publish "Ms.Payments.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio de Runtime
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Variáveis de ambiente
ENV RABBITMQ_HOST="rabbitmq-server"
ENV RABBITMQ_USERNAME="guest"
ENV RABBITMQ_PASSWORD="guest"

ENTRYPOINT ["dotnet", "Ms.Payments.Worker.dll"]