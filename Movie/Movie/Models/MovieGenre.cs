using MovieDatabase;

namespace Movie.Models;

/// <summary>
/// A representation of a single movie genre.
/// This differs from <see cref="MovieGenres"/>, only ever
/// representing a single genre. This also doesn't need to
/// have a 'none' genre, making business logic easier to work with!
/// </summary>
public enum MovieGenre
{
    Action,
    Adventure,
    ScienceFiction,
    Crime,
    Mystery,
    Thriller,
    Animation,
    Comedy,
    Family,
    Fantasy,
    War,
    Horror,
    Drama,
    Music,
    Romance,
    Western,
    History,
    TvMovie,
    Documentary,
}