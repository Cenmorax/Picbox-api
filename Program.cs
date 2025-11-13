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
        // Try to open a connection explicitly to get the real error
        await db.Database.OpenConnectionAsync();
        await db.Database.CloseConnectionAsync();

        return Results.Ok(new
        {
            success = true,
            message = "Database connected successfully!"
        });
    }
    catch (Exception ex)
    {
        return Results.Ok(new
        {
            success = false,
            error = ex.Message,
            innerError = ex.InnerException?.Message,
            type = ex.GetType().Name
        });
    }
});




app.Run();
