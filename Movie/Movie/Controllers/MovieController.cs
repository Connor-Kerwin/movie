using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie.Models;
using MovieDatabase;

namespace Movie.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly MovieDbContext db;
    
    public MovieController(MovieDbContext db)
    {
        this.db = db;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? title, 
        [FromQuery] int? pageSize,
        [FromQuery] int? page)
    {
        var actualPage = page ?? 0;
        var actualPageSize = pageSize ?? 0;
        
        var position = actualPage * actualPageSize;

        var results = await db.Movies
            .Skip(position)
            .Take(actualPageSize)
            .ToListAsync();

        var resultModels = new List<MovieModel>(results.Count);
        foreach (var entity in results)
        {
            var model = new MovieModel(entity);
            resultModels.Add(model);
        }

        return Ok(resultModels);
    }

    [HttpGet("Test")]
    public async Task<IActionResult> Test()
    {
        var entity = await db.Movies.FirstOrDefaultAsync();
        var model = new MovieModel(entity);
        
        return Ok(model);
    }
}