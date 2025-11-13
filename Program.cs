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

app.MapGet("/api/db-test", async (AppDbContext db, IConfiguration config) =>
{
    try
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        var canConnect = await db.Database.CanConnectAsync();
        return Results.Ok(new
        {
            databaseConnected = canConnect,
            message = "Connection successful!"
        });
    }
    catch (Exception ex)
    {
        return Results.Ok(new
        {
            databaseConnected = false,
            error = ex.Message,
            innerError = ex.InnerException?.Message,
            stackTrace = ex.StackTrace?.Substring(0, Math.Min(500, ex.StackTrace.Length))
        });
    }
});



app.Run();
