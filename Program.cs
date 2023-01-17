using BlazorApp.Data;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

Uri KeyVaultUri = new Uri("https://testthf-keyvault.vault.azure.net/");
var default_credential = new DefaultAzureCredential();
var keyVaultClient = new SecretClient(KeyVaultUri, default_credential);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<Azure.Core.TokenCredential>(default_credential);
builder.Services.AddSingleton<SecretClient>(keyVaultClient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

var temp = keyVaultClient.GetSecretAsync("7a247311-5d2a-4d13-8187-2e6fd9e17994");
temp = default!;

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
