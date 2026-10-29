var builder = WebApplication.CreateBuilder(args);

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5272", "http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


// Enregistrement des clients HTTP pour cibler les deux microservices
builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri("http://authentification_api:5157/");
});

builder.Services.AddHttpClient("GestionAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5130/");
});

var app = builder.Build();

// Activation du middleware CORS
app.UseCors("AllowBlazorClient");

app.MapReverseProxy();

// Route de redirection du Login vers AuthentificationApi (Port 5097)
app.MapPost("/api/Authentication/login", async (IHttpClientFactory clientFactory, HttpContext context) =>
{
    var client = clientFactory.CreateClient("AuthApi");

    // Lecture sécurisée du body JSON
    using var reader = new StreamReader(context.Request.Body);
    var bodyString = await reader.ReadToEndAsync();

    var request = new HttpRequestMessage(HttpMethod.Post, "api/Authentication/login")
    {
        Content = new StringContent(bodyString, System.Text.Encoding.UTF8, "application/json")
    };

    var response = await client.SendAsync(request);

    context.Response.StatusCode = (int)response.StatusCode;
    context.Response.ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";
    
    var responseBody = await response.Content.ReadAsStringAsync();
    await context.Response.WriteAsync(responseBody);
});

// Route de proxy générique vers GestionOrdinateurs.API (Port 5130)
app.Map("{*path}", async (string path, IHttpClientFactory clientFactory, HttpContext context) =>
{
    if (!path.StartsWith("api/")) return;

    var client = clientFactory.CreateClient("GestionAPI");
    var request = new HttpRequestMessage(new HttpMethod(context.Request.Method), path + context.Request.QueryString);

    if (context.Request.Method != "GET" && context.Request.Method != "HEAD")
    {
        using var reader = new StreamReader(context.Request.Body);
        var bodyString = await reader.ReadToEndAsync();
        request.Content = new StringContent(bodyString, System.Text.Encoding.UTF8, "application/json");
    }

    foreach (var header in context.Request.Headers)
    {
        if (header.Key.StartsWith("Authorization", StringComparison.OrdinalIgnoreCase))
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }
    }

    var response = await client.SendAsync(request);

    context.Response.StatusCode = (int)response.StatusCode;
    context.Response.ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";
    
    var responseBody = await response.Content.ReadAsStringAsync();
    await context.Response.WriteAsync(responseBody);
});

app.Run();