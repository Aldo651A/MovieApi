using MovieApi.DTOs;
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


    public async Task<Movie> CreateMovieAsync(CreateMovieDto data)
    {
        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            """
            INSERT INTO movies (title, director, release_year)
            VALUES (@title, @director, @year);
            """,
            connection
        );

        command.Parameters.AddWithValue("@title", data.Title);
        command.Parameters.AddWithValue("@director", data.Director);
        command.Parameters.AddWithValue("@year", data.ReleaseYear);

        await command.ExecuteNonQueryAsync();

        int id = checked((int)command.LastInsertedId);

        return new Movie(
            id,
            data.Title,
            data.Director,
            data.ReleaseYear
        );
    }


    public async Task<bool> DeleteMovieAsync(int id)
    {
        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "DELETE FROM movies WHERE id = @id;",
            connection
        );

        command.Parameters.AddWithValue("@id", id);

        int affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }

}

