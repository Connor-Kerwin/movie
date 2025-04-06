using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie.Models;
using MovieDatabase;
using Swashbuckle.AspNetCore.Annotations;

namespace Movie.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly MovieDbContext db;

    public MoviesController(MovieDbContext db)
    {
        this.db = db;
    }

    /// <summary>
    /// Get a paginated set of movies based on the given query parameters.
    /// </summary>
    /// <param name="parameters">A set of parameters to control the search behaviour.</param>
    /// <param name="cancellation">The means to cancel the request.</param>
    /// <returns></returns>
    [HttpGet]
    [Produces("application/json")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(PaginatedMoviesModel))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, Type = typeof(ErrorModel))]
    public async Task<IActionResult> GetMovies([FromQuery] SearchModel parameters, CancellationToken cancellation)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var query = BuildQuery(parameters);

        var count = await query.CountAsync(cancellation);

        var position = parameters.Page * parameters.PageSize;

        var entities = await query
            .Skip(position)
            .Take(parameters.PageSize)
            .ToListAsync(cancellation);

        var result = new PaginatedMoviesModel
        {
            Pagination =
            {
                Page = parameters.Page,
                PageSize = parameters.PageSize,
                TotalRecords = count
            }
        };

        // Build data models to return
        foreach (var entity in entities)
        {
            var model = new MovieModel(entity);
            result.Records.Add(model);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get a movie based on its unique id. This is mostly included as a nice to have functionality. Depending on the use case, this endpoint might not want to exist!
    /// </summary>
    /// <param name="id">The id of the movie to search for.</param>
    /// <param name="cancellation">The means to cancel the request.</param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    [Produces("application/json")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(MovieModel))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, Type = typeof(ErrorModel))]
    public async Task<IActionResult> GetMovie(int id, CancellationToken cancellation)
    {
        var entity = await db.Movies.FindAsync(id, cancellation);

        if (entity == null)
        {
            return NotFound();
        }
        
        var model = new MovieModel(entity);
        return Ok(model);
    }
    
    /// <summary>
    /// Get a list of possible genres.
    /// </summary>
    /// <returns></returns>
    [HttpGet("genres")]
    [Produces("application/json")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<MovieGenre>))]
    public IActionResult GetGenres()
    {
        var values = Enum.GetValues<MovieGenre>();
        var names = new List<MovieGenre>(values.Length);

        foreach (var value in values)
        {
            names.Add(value);
        }

        return Ok(names);
    }
    
    private IQueryable<MovieEntity> BuildQuery(SearchModel parameters)
    {
        var genreFilter = ExtractGenreFilter(parameters);

        var request = db.Movies
            .AsNoTracking();

        // Optionally apply the genre filter
        if (genreFilter != MovieGenreFlags.None)
        {
            request = request.Where(m => (m.Genres & genreFilter) == genreFilter);
        }

        // Apply sorting and ordering
        switch (parameters.SortBy)
        {
            case SortBy.Title:
            {
                switch (parameters.OrderBy)
                {
                    case OrderBy.Asc:
                    {
                        request = request.OrderBy(m => m.Title);
                    }
                        break;
                    case OrderBy.Desc:
                    {
                        request = request.OrderByDescending(m => m.Title);
                    }
                        break;
                }
            }
                break;
            case SortBy.ReleaseDate:
            {
                switch (parameters.OrderBy)
                {
                    case OrderBy.Asc:
                    {
                        request = request.OrderBy(m => m.ReleaseDate);
                    }
                        break;
                    case OrderBy.Desc:
                    {
                        request = request.OrderByDescending(m => m.ReleaseDate);
                    }
                        break;
                }
            }
                break;
        }

        return request;
    }

    private static MovieGenreFlags ExtractGenreFilter(SearchModel model)
    {
        if (model.Genres == null)
        {
            return MovieGenreFlags.None;
        }

        return GenreMapper.ComposeGenreFlags(model.Genres);
    }
}