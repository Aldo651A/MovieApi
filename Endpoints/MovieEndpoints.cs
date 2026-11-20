using MovieApi.Models;
using MovieApi.Repositories;

namespace MovieApi.Endpoints;

public static class MovieEndpoints
{
    public static void MapMovieEndpoints(this WebApplication app)
    {
        var movies = app.MapGroup("/api/movies");

        movies.MapGet("", async (MovieRepository repository) =>
        {
            List<Movie> result =
                await repository.GetMoviesAsync();

            return Results.Ok(result);
        });
    }
}
