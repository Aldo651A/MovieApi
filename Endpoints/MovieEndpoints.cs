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
    

            movies.MapGet("/{id}", async (
            int id,
            MovieRepository repository) =>
        {
            if (id <= 0)
            {
                return Results.BadRequest(new
                {
                    message = "ID must be greater than zero."
                });
            }

            Movie? movie = await repository.GetMovieAsync(id);

            if (movie is null)
            {
                return Results.NotFound(new
                {
                    message = "Movie not found."
                });
            }

            return Results.Ok(movie);
        });
    
    }

}
