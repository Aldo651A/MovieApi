using MovieApi.Endpoints;
using MovieApi.Repositories;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<MovieRepository>();

var app = builder.Build();

app.MapGet("/", () => "Welcome to Movie API!");

app.MapGet("/api/health/db", async (
    IConfiguration configuration,
    ILogger<Program> logger) =>
{
    string connectionString =
        configuration.GetConnectionString("MovieDatabase")
        ?? throw new InvalidOperationException(
            "Missing database connection string.");

    try
    {
        await using var connection =
            new MySqlConnection(connectionString);

        await connection.OpenAsync();

        await using var command =
            new MySqlCommand("SELECT 1;", connection);

        object? result = await command.ExecuteScalarAsync();

        return Results.Ok(new
        {
            message = "Database connection successful",
            result
        });
    }
    catch (MySqlException ex)
    {
        logger.LogError(ex, "Database connection failed");

        return Results.Problem(
            title: "Database unavailable",
            statusCode: 503
        );
    }
});

app.MapMovieEndpoints();

app.Run();
