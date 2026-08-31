using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WEB_453504_ASP_NET.API;
using WEB_453504_ASP_NET.API.Data;
using WEB_453504_ASP_NET.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AppDbContext>());
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowKeycloak", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.AddSwaggerWithKeycloak();

var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var authServer = builder.Configuration.GetSection("Keycloak").Get<AuthServerData>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
{
    o.MetadataAddress = $"{authServer.Host}/realms/{authServer.Realm}/.well-known/openid-configuration";
    o.Authority = $"{authServer.Host}/realms/{authServer.Realm}";
    o.Audience = "account";
    o.RequireHttpsMetadata = false;
});
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p => p.RequireRole("POWER-USER"));
});

builder.Services.AddHybridCache();
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.InstanceName = "labs_";
    opt.Configuration = builder
    .Configuration
    .GetConnectionString("Redis");
});

var app = builder.Build();
await DbInitializer.SeedData(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger().UseSwaggerUI(options =>
    {
        options.OAuthClientId(builder.Configuration["Keycloak:ClientId"]!); 
        options.OAuthClientSecret(builder.Configuration["Keycloak:ClientSecret"]!);
        options.OAuthUsePkce();
    });
    app.MapGet("/", async (context) => context.Response.Redirect("/swagger"));
}

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowKeycloak");

app.MapControllers();
app.UseStaticFiles();
app.MapStaticAssets();
app.UseHttpsRedirection();

app.MapMusicalInstrumentEndpoints();

app.Run();
