using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using WEB_453504_ASP_NET.API.Data;
using WEB_453504_ASP_NET.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AppDbContext>());

var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();
DbInitializer.SeedData(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger().UseSwaggerUI();
    app.MapGet("/", async (context) => context.Response.Redirect("/swagger"));
}

app.MapControllers();
app.UseStaticFiles();
app.MapStaticAssets();
app.UseHttpsRedirection();

app.MapMusicalInstrumentEndpoints();

app.Run();
