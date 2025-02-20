FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GHPAT
ARG VERSION
WORKDIR /src
COPY ["Texnokaktus.ProgOlymp.Data/Texnokaktus.ProgOlymp.Data.csproj", "Texnokaktus.ProgOlymp.Data/"]
COPY ["Texnokaktus.ProgOlymp.Data.Infrastructure/Texnokaktus.ProgOlymp.Data.Infrastructure.csproj", "Texnokaktus.ProgOlymp.Data.Infrastructure/"]
COPY ["modules/Common.Contracts/Texnokaktus.ProgOlymp.Common.Contracts/Texnokaktus.ProgOlymp.Common.Contracts.csproj", "modules/Common.Contracts/Texnokaktus.ProgOlymp.Common.Contracts/"]
RUN dotnet nuget add source --username kav128 --password $GHPAT --store-password-in-clear-text --name "GitHub Texnokaktus-Progolymp" https://nuget.pkg.github.com/Texnokaktus-ProgOlymp/index.json
RUN dotnet restore "Texnokaktus.ProgOlymp.Data/Texnokaktus.ProgOlymp.Data.csproj"
COPY . .
WORKDIR "/src/Texnokaktus.ProgOlymp.Data"
RUN dotnet build "Texnokaktus.ProgOlymp.Data.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Texnokaktus.ProgOlymp.Data.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:Version=$VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Texnokaktus.ProgOlymp.Data.dll"]
