$root="C:\Users\Yannick\Documents\EFREI 26-27\DOTNET C#\GestionOrdinateurs"; @(
    "$root\GestionOrdinateurs.API\GestionOrdinateurs.API.csproj",
    "$root\AuthentificationApi\AuthentificationApi.csproj",
    "$root\GestionOrdinateurs.Gateway\GestionOrdinateurs.Gateway.csproj",
    "$root\GestionOrdinateurs.Client\GestionOrdinateurs.Client.csproj"
) | ForEach-Object { dotnet clean $_ }

$root="C:\Users\Yannick\Documents\EFREI 26-27\DOTNET C#\GestionOrdinateurs"; Start-Process dotnet -ArgumentList "run --project `"$root\GestionOrdinateurs.API\GestionOrdinateurs.API.csproj`" --no-launch-profile --urls http://localhost:5210"; Start-Process dotnet -ArgumentList "run --project `"$root\AuthentificationApi\AuthentificationApi.csproj`" --no-launch-profile --urls http://localhost:5114"; Start-Process dotnet -ArgumentList "run --project `"$root\GestionOrdinateurs.Gateway\GestionOrdinateurs.Gateway.csproj`" --no-launch-profile --urls http://localhost:5010"; Start-Process dotnet -ArgumentList "run --project `"$root\GestionOrdinateurs.Client\GestionOrdinateurs.Client.csproj`" --no-launch-profile --urls http://localhost:5170"

