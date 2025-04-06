using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie.Models;
using MovieDatabase;

namespace Movie.Controllers;

[ApiController]
[Route("api/movies")]
public class MovieController : ControllerBase
{
    private readonly MovieDbContext db;

    public MovieController(MovieDbContext db)
    {
        this.db = db;
    }

    [HttpGet("{title}")]
    public async Task<IActionResult> GetMovie([FromRoute] string title)
    {
        var entity = await db.Movies
            .Where(m => m.Title == title)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return NotFound();
        }

        var model = new MovieModel(entity);
        return Ok(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetMovies([FromQuery] SearchModel parameters)
    {
        var results = await GetMoviesAsync(parameters);

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

    private async Task<List<MovieEntity>> GetMoviesAsync(SearchModel model)
    {
        var position = model.Page.Value * model.PageSize.Value;

        var results = await db.Movies
            .Skip(position)
            .Take(model.PageSize.Value)
            .ToListAsync();

        return results;
    }
}

public class SearchModel
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? Title { get; set; }
}