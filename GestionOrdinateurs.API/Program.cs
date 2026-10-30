using System.Text;
using GestionOrdinateurs.API.Data;
using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGatewayAndClient", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "http://localhost:5272")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configuration JWT Keycloak
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Adresse interne Docker de Keycloak
    options.Authority = builder.Configuration["Keycloak:Authority"] ?? "http://keycloak:8080/realms/parcinfo-realm";
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false
    };
});

// Services métier & BDD
builder.Services.AddScoped<IOrdinateurService, OrdinateurService>();
builder.Services.AddDbContext<ParcInformatiqueContext>(options =>
    options.UseSqlite("Data Source=parcinfo.db"));
builder.Services.AddScoped<ReparationService>();
builder.Services.AddSingleton<StockService>();
builder.Services.AddSingleton<JournalService>();

var app = builder.Build();

// Seeding BDD
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowGatewayAndClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();