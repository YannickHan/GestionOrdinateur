FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY GestionOrdinateurs.slnx .
COPY AuthentificationApi/AuthentificationApi.csproj AuthentificationApi/
COPY GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj GestionOrdinateurs.Shared/

RUN dotnet restore AuthentificationApi/AuthentificationApi.csproj

COPY AuthentificationApi/ AuthentificationApi/
COPY GestionOrdinateurs.Shared/ GestionOrdinateurs.Shared/

RUN dotnet publish AuthentificationApi/AuthentificationApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AuthentificationApi.dll"]
