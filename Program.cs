using System.Globalization;
using catalogo_filmes_previsao_tempo.Data;
using catalogo_filmes_previsao_tempo.Services;
using catalogo_filmes_previsao_tempo.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFile("logs/app.log");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ITmdbApiService, TmdbApiService>();
builder.Services.AddScoped<ITmdbApiService, TmdbApiService>();
builder.Services.AddScoped<IFilmeRepository, FilmeRepository>();

builder.Services.AddHttpClient<IWeatherApiService, WeatherApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
var defaultCulture = new CultureInfo("pt-BR");
var supportedCultures = new[] { defaultCulture };
    
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();