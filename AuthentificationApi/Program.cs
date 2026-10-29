var builder = WebApplication.CreateBuilder(args);

// Configuration CORS unifiée
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5272",   // Blazor en HTTP
                "https://localhost:5272",  // Blazor en HTTPS
                "http://localhost:5001",
                "https://localhost:5001",
                "http://localhost:5000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Ajout des services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClientFactory pour appeler GestionOrdinateurs.API
builder.Services.AddHttpClient("GestionOrdinateursAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5130/");
});

var app = builder.Build();

// Activation de Swagger en mode développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activation du CORS (Impérativement avant UseAuthorization et UseHttpsRedirection)
app.UseCors("AllowBlazorClient");

app.UseAuthorization();

app.MapControllers();

app.Run();