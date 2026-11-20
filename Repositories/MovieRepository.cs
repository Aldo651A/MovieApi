using MovieApi.Models;
using MySqlConnector;

namespace MovieApi.Repositories;

public class MovieRepository
{
    private readonly string _connectionString;

    public MovieRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("MovieDatabase")
            ?? throw new InvalidOperationException(
                "Missing database connection string.");
    }

    public async Task<List<Movie>> GetMoviesAsync()
    {
        List<Movie> movies = [];

        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "SELECT id, title, director, release_year FROM movies;",
            connection
        );

        await using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Movie movie = new(
                reader.GetInt32("id"),
                reader.GetString("title"),
                reader.GetString("director"),
                reader.GetInt32("release_year")
            );

            movies.Add(movie);
        }

        return movies;
    }


    public async Task<Movie?> GetMovieAsync(int id)
    {
        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            """
            SELECT id, title, director, release_year
            FROM movies
            WHERE id = @id;
            """,
            connection
        );

        command.Parameters.AddWithValue("@id", id);

        await using var reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Movie(
                reader.GetInt32("id"),
                reader.GetString("title"),
                reader.GetString("director"),
                reader.GetInt32("release_year")
            );
        }

        return null;
    }

}
