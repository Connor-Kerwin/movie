using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Movie.Models;
using MovieDatabase;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

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
        var values = Enum.GetValues(typeof(MovieGenres));
        var names = new List<string>();

        foreach (var value in values)
        {
            var genreValue = (MovieGenres)value;
            if (genreValue == MovieGenres.None)
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
        // TODO: Genre validation can maybe run automatically as a custom validator!
        var genreFilter = parameters.ValidateGenre(ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var results = await GetMoviesAsync(parameters, genreFilter);

        var resultModels = new List<MovieModel>(results.Count);
        foreach (var entity in results)
        {
            var model = new MovieModel(entity);
            resultModels.Add(model);
        }


        // HACK
        return Ok(resultModels);

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

    private async Task<List<MovieEntity>> GetMoviesAsync(SearchModel model, MovieGenres genreFilter)
    {
        var position = model.Page * model.PageSize;

        var request = db.Movies
            .AsNoTracking();

        // Optionally apply the genre filter
        if (genreFilter != MovieGenres.None)
        {
            request = request.Where(m => (m.Genre & genreFilter) == genreFilter);
        }

        return await request
            .Skip(position)
            .Take(model.PageSize)
            .ToListAsync();
    }

    private MovieGenres ExtractGenreFilter(SearchModel model)
    {
        if (model.Genres == null)
        {
            return MovieGenres.None;
        }

        var result = MovieGenres.None;

        foreach (var str in model.Genres)
        {
            ModelState.AddModelError("", "");
        }

        return result;
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
    [EnumDataType(typeof(SortMode))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortMode SortBy { get; set; } = SortMode.Title;

    [MaxLength(32)]
    [FromQuery(Name = "genres")]
    public HashSet<string>? Genres { get; set; }

    public MovieGenres ValidateGenre(ModelStateDictionary modelState)
    {
        if (Genres == null)
        {
            return MovieGenres.None;
        }

        var result = MovieGenres.None;

        foreach (var str in Genres)
        {
            if (!GenreMapper.ParseSingleGenre(str, out var singleGenre))
            {
                modelState.AddModelError("genres", $"{str} is not a valid genre");
                continue;
            }

            var flagsGenre = GenreMapper.GetFlagsGenre(singleGenre);
            result |= flagsGenre;
        }

        return result;
    }
}