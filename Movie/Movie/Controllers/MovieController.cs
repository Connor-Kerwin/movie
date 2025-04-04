using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie.Db;

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
    public IActionResult Test()
    {
        var t = db.Movies.Take(1);
        return Ok(t);
    }
}