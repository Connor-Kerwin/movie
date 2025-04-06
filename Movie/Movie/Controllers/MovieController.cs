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

    [HttpGet("Test")]
    public async Task<IActionResult> Test()
    {
        var entity = await db.Movies.FirstOrDefaultAsync();
        var model = new MovieModel(entity);
        
        return Ok(model);
    }
}