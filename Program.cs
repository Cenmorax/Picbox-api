using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello from ASP.NET API on Render!");

app.MapGet("/api/test", () => new { message = "This is a test endpoint", status = "success" });

app.MapGet("/api/db-test", () => async (AppDbContext db) =>
{
    var canConnect = await db.Database.CanConnectAsync();
    return new { databaseConnected = canConnect };
});

app.Run();
