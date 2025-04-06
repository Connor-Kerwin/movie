using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie.Models;
using MovieDatabase;

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

    [HttpGet]
    public async Task<IActionResult> GetMovies([FromQuery] SearchModel parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var query = BuildQuery(parameters);

        var count = await query.CountAsync();
        
        var position = parameters.Page * parameters.PageSize;
        
        var entities = await query
            .Skip(position)
            .Take(parameters.PageSize)
            .ToListAsync();
        
        var result = new PaginatedMoviesModel()
        {
            Pagination = new PageInfoModel()
            {
                PageSize = parameters.PageSize,
                Page = parameters.Page,
                TotalRecords = count // TODO: Pull this as part of the query
            },
            Records = new List<MovieModel>()
        };

        foreach (var entity in entities)
        {
            var model = new MovieModel(entity);
            result.Records.Add(model);
        }

        //return result;

        // var result = await GetMoviesAsync(parameters);
        return Ok(result);
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
                if (parameters.OrderBy == OrderBy.Asc)
                {
                    request = request.OrderBy(m => m.Title);
                }
                else
                {
                    request = request.OrderByDescending(m => m.Title);
                }
            }
                break;
            case SortBy.ReleaseDate:
            {
                if (parameters.OrderBy == OrderBy.Asc)
                {
                    request = request.OrderBy(m => m.ReleaseDate);
                }
                else
                {
                    request = request.OrderByDescending(m => m.ReleaseDate);
                }
            }
                break;
        }

        // var entities = await request
        //     .GroupBy(m => true)
        //     .Select(g => new
        //     {
        //         // NOTE: As a single transaction, pull out the total number of items
        //
        //         Total = g.Count(),
        //         Records = g.Skip(position).Take(parameters.PageSize).AsEnumerable()
        //     })
        //     .FirstOrDefaultAsync();

        return request;
    }

    // [HttpGet]
    // public async Task<IActionResult> GetMovies([FromQuery] SearchModel parameters)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }
    //
    //     var position = parameters.Page * parameters.PageSize;
    //     var genreFilter = ExtractGenreFilter(parameters);
    //
    //     var request = db.Movies
    //         .AsNoTracking();
    //
    //     // Optionally apply the genre filter
    //     if (genreFilter != MovieGenreFlags.None)
    //     {
    //         request = request.Where(m => (m.Genres & genreFilter) == genreFilter);
    //     }
    //     
    //     // Apply the ordering
    //     switch (parameters.SortBy)
    //     {
    //         case SortMode.Title:
    //         {
    //             request = request.OrderBy(m => m.Title);
    //         }
    //             break;
    //         case SortMode.ReleaseDate:
    //         {
    //             request = request.OrderBy(m => m.ReleaseDate);
    //         }
    //             break;
    //         default:
    //         {
    //             request = request.OrderBy(m => m.Id);
    //         }
    //             break;
    //     }
    //
    //     var entities = await request
    //         .GroupBy(m => true)
    //         .Select(g => new
    //         {
    //             // NOTE: As a single transaction, pull out the total number of items
    //             
    //             Total = g.Count(),
    //             Records = g.Skip(position).Take(parameters.PageSize).AsEnumerable()
    //         })
    //         .FirstOrDefaultAsync();
    //
    //     var result = new PaginatedMoviesModel()
    //     {
    //         Pagination = new PageInfoModel()
    //         {
    //             PageSize = parameters.PageSize,
    //             Page = parameters.Page,
    //             TotalRecords = entities.Total // TODO: Pull this as part of the query
    //         },
    //         Records = new List<MovieModel>()
    //     };
    //
    //     foreach (var entity in entities.Records)
    //     {
    //         var model = new MovieModel(entity);
    //         result.Records.Add(model);
    //     }
    //
    //     //return result;
    //     
    //    // var result = await GetMoviesAsync(parameters);
    //     return Ok(result);
    // }

    [HttpGet("genres")]
    public async Task<IActionResult> GetGenres()
    {
        var values = Enum.GetValues<MovieGenre>();
        var names = new List<MovieGenre>(values.Length);

        foreach (var value in values)
        {
            names.Add(value);
        }

        return Ok(names);
    }
    // private async Task<PaginatedMoviesModel> GetMoviesAsync(SearchModel parameters)
    // {
    //   
    // }


    private static MovieGenreFlags ExtractGenreFilter(SearchModel model)
    {
        if (model.Genres == null)
        {
            return MovieGenreFlags.None;
        }

        return GenreMapper.ComposeGenreFlags(model.Genres);
    }
}

public enum SortBy
{
    Title,
    ReleaseDate
}

public enum OrderBy
{
    Asc,
    Desc
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
    [ModelBinder(BinderType = typeof(KebabCaseEnumModelBinder<SortBy>))]
    public SortBy SortBy { get; set; } = SortBy.Title;

    [FromQuery(Name = "orderby")]
    [ModelBinder(BinderType = typeof(KebabCaseEnumModelBinder<OrderBy>))]
    public OrderBy OrderBy { get; set; } = OrderBy.Asc;
    
    [MaxLength(32)]
    [FromQuery(Name = "genres")]
    [ModelBinder(BinderType = typeof(KebabCaseEnumModelBinder<MovieGenre>))]
    public HashSet<MovieGenre>? Genres { get; set; }
}