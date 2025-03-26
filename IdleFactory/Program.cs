using System.Globalization;
using IdleFactory;
using IdleFactory.Components;
using IdleFactory.State;
using IdleFactory.Util;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

//CultureInfo.CurrentCulture = new CultureInfo("zh");


SingletonHolder.GetSingleton<GameEntry>();


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
builder.Services.AddScoped<ICookie, Cookie>();
builder.Services.AddControllers();

var app = builder.Build();

string[] supportedCultures = ["en-US", "zh"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();