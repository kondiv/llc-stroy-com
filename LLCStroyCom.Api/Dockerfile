FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["LLCStroyCom.sln", "./"]
COPY ["LLCStroyCom.Api/LLCStroyCom.Api.csproj", "LLCStroyCom.Api/"]
COPY ["LLCStroyCom.Application/LLCStroyCom.Application.csproj", "LLCStroyCom.Application/"]
COPY ["LLCStroyCom.Domain/LLCStroyCom.Domain.csproj", "LLCStroyCom.Domain/"]
COPY ["LLCStroyCom.Infrastructure/LLCStroyCom.Infrastructure.csproj", "LLCStroyCom.Infrastructure/"]
COPY ["LLCStroyCom.Tests/LLCStroyCom.Tests.csproj", "LLCStroyCom.Tests/"]

RUN dotnet restore "LLCStroyCom.sln"

COPY . .

ARG BUILD_CONFIGURATION=Release
RUN dotnet publish LLCStroyCom.Api/LLCStroyCom.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LLCStroyCom.Api.dll"]
