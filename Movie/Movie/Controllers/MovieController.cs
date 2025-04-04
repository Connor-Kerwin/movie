using Microsoft.AspNetCore.Mvc;
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
        //return Ok("Hello");
        
        var t = db.Movies.FirstOrDefault();
        return Ok(t);
    }
}