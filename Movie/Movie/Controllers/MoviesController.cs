using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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

    [HttpGet("genres")]
    public async Task<IActionResult> GetGenres()
    {
        var values = Enum.GetValues(typeof(MovieGenreFlags));
        var names = new List<string>();

        foreach (var value in values)
        {
            var genreValue = (MovieGenreFlags)value;
            if (genreValue == MovieGenreFlags.None)
            {
                continue;
            }

            names.Add(genreValue.ToString().ToLower());
        }

        return Ok(names);
    }

    [HttpGet]
    public async Task<IActionResult> GetMovies([FromQuery] SearchModel parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await GetMoviesAsync(parameters);
        return Ok(result);
        //
        // var resultModels = new List<MovieModel>(results.Count);
        // foreach (var entity in results)
        // {
        //     var model = new MovieModel(entity);
        //     resultModels.Add(model);
        // }
        //
        //
        // // HACK
        // return Ok(resultModels);

        //
        // var results = await db.Movies
        //     .Skip(position)
        //     .Take(actualPageSize)
        //     .ToListAsync();
        //
        // var resultModels = new List<MovieModel>(results.Count);
        // foreach (var entity in results)
        // {
        //     var model = new MovieModel(entity);
        //     resultModels.Add(model);
        // }
        //
        // return Ok(resultModels);
    }

    private async Task<PaginatedMoviesModel> GetMoviesAsync(SearchModel parameters)
    {
        var position = parameters.Page * parameters.PageSize;
        var genreFilter = ExtractGenreFilter(parameters);

        var request = db.Movies
            .AsNoTracking();

        // Apply the ordering
        switch (parameters.SortBy)
        {
            case SortMode.Title:
            {
                request = request.OrderBy(m => m.Title);
            }
                break;
            case SortMode.ReleaseDate:
            {
                request = request.OrderBy(m => m.ReleaseDate);
            }
                break;
        }

        // Optionally apply the genre filter
        if (genreFilter != MovieGenreFlags.None)
        {
            request = request.Where(m => (m.Genres & genreFilter) == genreFilter);
        }

        var entities = await request
            .Skip(position)
            .Take(parameters.PageSize)
            .ToListAsync();

        var result = new PaginatedMoviesModel()
        {
            Pagination = new PageInfoModel()
            {
                PageSize = parameters.PageSize,
                Page = parameters.Page,
                TotalRecords = 1337 // TODO: Pull this as part of the query
            },
            Records = new List<MovieModel>()
        };

        foreach (var entity in entities)
        {
            var model = new MovieModel(entity);
            result.Records.Add(model);
        }

        return result;
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

public enum SortMode
{
    Title,
    ReleaseDate
}

public class SearchModel
{
    [Required]
    [Range(0, int.MaxValue)]
    [FromQuery(Name = "page")]
    public int Page { get; set; }

    [Required]
    [Range(1, 64)]
    [FromQuery(Name = "pagesize")]
    public int PageSize { get; set; }

    [FromQuery(Name = "sortby")]
    [ModelBinder(BinderType = typeof(KebabCaseEnumModelBinder<SortMode>))]
    public SortMode SortBy { get; set; } = SortMode.Title;

    [MaxLength(32)]
    [FromQuery(Name = "genres")]
    [ModelBinder(BinderType = typeof(KebabCaseEnumModelBinder<MovieGenre>))]
    public HashSet<MovieGenre>? Genres { get; set; }
}