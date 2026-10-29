using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.services.interfaces;
using GestionOrdinateurs.API.contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IOrdinateurService, OrdinateurService>();
builder.Services.AddSingleton<JournalService>();
// Enregistre le service qui gère les réparations.
builder.Services.AddScoped<IReparationService, ReparationService>();
// Utilise une seule instance du stock SSD pour partager sa valeur entre les requêtes.
builder.Services.AddSingleton<StockService>();
builder.Services.AddDbContext<ParcInformatiqueContext>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "http://localhost:5170", "https://localhost:7290")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GestionOrdinateurs",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez : Bearer {token}"
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Keycloak:Authority"]
        ?? "http://keycloak:8080/realms/gestion-ordinateurs";
    options.MetadataAddress = builder.Configuration["Keycloak:MetadataAddress"]
        ?? "http://keycloak:8080/realms/gestion-ordinateurs/.well-known/openid-configuration";
    options.RequireHttpsMetadata = false;
    options.BackchannelHttpHandler = new KeycloakBackchannelHandler();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKeyResolver = (_, _, _, _) =>
        {
            using var httpClient = new HttpClient();
            var jwks = httpClient.GetStringAsync(
                "http://keycloak:8080/realms/gestion-ordinateurs/protocol/openid-connect/certs")
                .GetAwaiter()
                .GetResult();
            return new JsonWebKeySet(jwks).GetSigningKeys();
        }
    };
    options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var realmAccess = context.Principal?.FindFirst("realm_access");
                if (realmAccess is not null)
                {
                    using var access = JsonDocument.Parse(realmAccess.Value);
                    if (access.RootElement.TryGetProperty("roles", out var roles))
                    {
                        var identity = context.Principal?.Identity as ClaimsIdentity;
                        foreach (var role in roles.EnumerateArray())
                        {
                            var roleName = role.GetString();
                            if (!string.IsNullOrWhiteSpace(roleName) &&
                                !context.Principal!.IsInRole(roleName))
                            {
                                identity?.AddClaim(new Claim(ClaimTypes.Role, roleName));
                            }
                        }
                    }
                }

                Console.WriteLine($"Utilisateur utilisant un Token Valide --- {context.Principal?.Identity?.Name ?? "Anonymous"}");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Token Invalide : {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
});
builder.Services.AddAuthorization();

var app = builder.Build();
    
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Initialise la base de données au lancement de l'API.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParcInformatiqueContext>();

    db.Database.Migrate();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Login = "user1", Role = RoleEnum.Admin },
            new User { Login = "user2", Role = RoleEnum.Gestionnaire },
            new User { Login = "user3", Role = RoleEnum.Gestionnaire }
        );

        db.SaveChanges();
    }
}

app.Run();

internal sealed class KeycloakBackchannelHandler : HttpClientHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri is not null &&
            request.RequestUri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) &&
            request.RequestUri.Port == 8180)
        {
            var builder = new UriBuilder(request.RequestUri)
            {
                Host = "keycloak",
                Port = 8080
            };
            request.RequestUri = builder.Uri;
        }

        return base.SendAsync(request, cancellationToken);
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
