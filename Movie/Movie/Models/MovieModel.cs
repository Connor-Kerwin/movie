using MovieDatabase;

namespace Movie.Models;

public class MovieModel
{
    /// <summary>
    /// The unique id for the movie.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the movie.
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// A brief summary of the movie.
    /// </summary>
    public string Overview { get; set; }
    
    /// <summary>
    /// A score of popularity of the movie.
    /// </summary>
    public float Popularity { get; set; }
    
    /// <summary>
    /// The Url of the poster for the movie.
    /// </summary>
    public string PosterUrl { get; set; }
    
    /// <summary>
    /// The total number of votes received from the viewers.
    /// </summary>
    public int VoteCount { get; set; }
    
    /// <summary>
    /// The average voted rating for the movie (out of 10).
    /// </summary>
    public double VoteAverage { get; set; }
    
    /// <summary>
    /// The date that the movie was released.
    /// </summary>
    public DateTime ReleaseDate { get; set; }
    
    /// <summary>
    /// The original language of the movie.
    /// </summary>
    public string OriginalLanguage { get; set; }
    
    /// <summary>
    /// A set of genres that the move is classified under.
    /// </summary>
    public List<MovieGenre> Genres { get; } = [];

    public MovieModel(MovieEntity entity)
    {
        Id = entity.Id;
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