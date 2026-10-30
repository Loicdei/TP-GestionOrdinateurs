using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using GestionOrdinateurs.Client;
using Blazored.SessionStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Service HTTP configuré pour inclure le token JWT
builder.Services.AddHttpClient("GatewayAPI", client => 
    client.BaseAddress = new Uri("http://localhost:5010/"))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// 2. Injection du HttpClient
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("GatewayAPI"));

builder.Services.AddBlazoredSessionStorage();

// 3. Authentification OIDC Keycloak
builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "http://localhost:8180/realms/parcinfo-realm";
    options.ProviderOptions.ClientId = "blazor-client";
    options.ProviderOptions.ResponseType = "code";
    
    options.UserOptions.NameClaim = "preferred_username";
    options.UserOptions.RoleClaim = "role";
});

await builder.Build().RunAsync();