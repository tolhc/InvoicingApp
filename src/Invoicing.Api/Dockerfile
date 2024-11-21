FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Invoicing.Api/Invoicing.Api.csproj", "src/Invoicing.Api/"]
COPY ["src/Invoicing.Application/Invoicing.Application.csproj", "src/Invoicing.Application/"]
COPY ["src/Invoicing.Core/Invoicing.Core.csproj", "src/Invoicing.Core/"]
COPY ["src/Invoicing.Infrastructure/Invoicing.Infrastructure.csproj", "src/Invoicing.Infrastructure/"]
RUN dotnet restore "src/Invoicing.Api/Invoicing.Api.csproj"
COPY . .
WORKDIR "/src/src/Invoicing.Api"
RUN dotnet build "Invoicing.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Invoicing.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Invoicing.Api.dll"]
