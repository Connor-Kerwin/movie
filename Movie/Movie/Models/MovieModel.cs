using MovieDatabase;

namespace Movie.Models;

public class PaginatedMoviesModel
{
    public PageInfoModel Pagination { get; } = new();
    public List<MovieModel> Records { get; } = new();
}

public class PageInfoModel
{
    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class MovieModel
{
    public string Title { get; set; }
    public string Overview { get; set; }
    public float? Popularity { get; set; }
    public string? PosterUrl { get; set; }
    public int? VoteCount { get; set; }
    public double? VoteAverage { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? OriginalLanguage { get; set; }

    public List<MovieGenre> Genres { get; } = [];

    public MovieModel(MovieEntity entity)
    {
        Title = entity.Title;
        Overview = entity.Overview;
        Popularity = entity.Popularity;
        PosterUrl = entity.PosterUrl;
        VoteCount = entity.VoteCount;
        VoteAverage = entity.VoteAverage;
        ReleaseDate = entity.ReleaseDate;
        OriginalLanguage = entity.OriginalLanguage;

        GenreMapper.ExtractIndividualGenres(entity.Genres, Genres);
    }
}