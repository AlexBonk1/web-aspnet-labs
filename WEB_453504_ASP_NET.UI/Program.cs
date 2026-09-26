using WEB_453504_ASP_NET.UI;
using WEB_453504_ASP_NET.UI.Models;
using WEB_453504_ASP_NET.UI.Services.Authentification;
using WEB_453504_ASP_NET.UI.Services.CategoryService;
using WEB_453504_ASP_NET.UI.Services.FileSevice;
using WEB_453504_ASP_NET.UI.Services.InstrumentService;
using WEB_453504_ASP_NET.UI.Services.Cart;
using WEB_453504_ASP_NET.UI.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



var uriData = builder.Configuration.GetSection("UriData").Get<UriData>();
builder.Services.Configure<KeycloakData>(builder.Configuration.GetSection("Keycloak"));
 
var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

builder.Host.UseSerilog(logger);

builder.Services.AddHttpClient<IInstrumentService, ApiInstrumentService>(opt => opt.BaseAddress = new Uri($"{uriData.ApiUri}/instruments/"));
builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(opt => opt.BaseAddress = new Uri($"{uriData.ApiUri}/categories/"));

logger.Information("Application started");

builder.Services.AddHttpClient<ITokenAccessor, KeycloakTokenAccessor>();
builder.Services.AddScoped<IFileService, LocalFileService>();
builder.Services.AddScoped<WEB_453504_ASP_NET.Domain.Entities.Cart, SessionCart>();
builder.Services.AddCookiesAuthentication(builder.Configuration);


builder.Services.AddAuthorization(opt =>
opt.AddPolicy("admin", p => p.RequireRole("POWER-USER")));

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaFolder("Admin", "/", "admin");
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
var app = builder.Build();

app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseStaticFiles();

// Добавляем middleware для логирования ошибок

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();


app.UseSession();

app.MapRazorPages().RequireAuthorization("admin");
app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();



app.Run();
