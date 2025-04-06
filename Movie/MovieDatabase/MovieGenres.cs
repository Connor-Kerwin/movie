namespace MovieDatabase;

/// <summary>
/// An enum flags representation of the different available genres of movie.
/// </summary>
[Flags]
public enum MovieGenres
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