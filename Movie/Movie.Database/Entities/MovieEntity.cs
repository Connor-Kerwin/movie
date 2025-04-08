namespace Movie.Database.Entities;

public class MovieEntity
{
    public int Id { get; set; }
    public MovieGenreFlags Genres { get; set; }
    public string Title { get; set; }
    public string Overview { get; set; }
    public float Popularity { get; set; }
    public string PosterUrl { get; set; }
    public int VoteCount { get; set; }
    public double VoteAverage { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string OriginalLanguage { get; set; }
}

/// <summary>
/// An enum flags representation of the different available genres of movie.
/// This enum is database-optimized, compressing all the known genres into bitflags.
/// </summary>
[Flags]
public enum MovieGenreFlags
{
    None = 0,
    Action = 1 << 0,
    Adventure = 1 << 1,
    ScienceFiction = 1 << 2,
    Crime = 1 << 3,
    Mystery = 1 << 4,
    Thriller = 1 << 5,
    Animation = 1 << 6,
    Comedy = 1 << 7,
    Family = 1 << 8,
    Fantasy = 1 << 9,
    War = 1 << 10,
    Horror = 1 << 11,
    Drama = 1 << 12,
    Music = 1 << 13,
    Romance = 1 << 14,
    Western = 1 << 15,
    History = 1 << 16,
    TvMovie = 1 << 17,
    Documentary = 1 << 18,
}