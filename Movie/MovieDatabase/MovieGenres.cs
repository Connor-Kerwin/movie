namespace MovieDatabase;

[Flags]
public enum MovieGenres
{
    None = 0,
    Action = 1,
    Adventure = 2 << 0,
    ScienceFiction = 2 << 1,
    Crime = 2 << 2,
    Mystery = 2 << 3,
    Thriller = 2 << 4,
    Animation = 2 << 5,
    Comedy = 2 << 6,
    Family = 2 << 7,
    Fantasy = 2 << 8,
    War = 2 << 9,
    Horror = 2 << 10,
    Drama = 2 << 11,
    Music = 2 << 12,
    Romance = 2 << 13,
    Western = 2 << 14,
    History = 2 << 15,
    TvMovie = 2 << 16,
    Documentary = 2 << 17,
}