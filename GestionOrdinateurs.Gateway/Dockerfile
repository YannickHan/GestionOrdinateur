FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY GestionOrdinateurs.slnx .
COPY GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj GestionOrdinateurs.Gateway/

RUN dotnet restore GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj

COPY GestionOrdinateurs.Gateway/ GestionOrdinateurs.Gateway/

RUN dotnet publish GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5010

ENTRYPOINT ["dotnet", "GestionOrdinateurs.Gateway.dll"]
