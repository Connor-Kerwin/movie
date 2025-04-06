namespace Movie.Models;

public class PaginatedMoviesModel
{
    /// <summary>
    /// A document containing details of the pagination.
    /// </summary>
    public PageInfoModel Pagination { get; } = new();
    
    /// <summary>
    /// A list of records for the requested page of data.
    /// </summary>
    public List<MovieModel> Records { get; } = new();
}