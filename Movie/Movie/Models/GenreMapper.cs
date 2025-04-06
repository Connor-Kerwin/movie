using MovieDatabase;

namespace Movie.Models;

/// <summary>
/// A utility class which facilitates the process of converting between <see cref="MovieGenre"/> and <see cref="MovieGenreFlags"/>.
/// </summary>
public static class GenreMapper
{
    public static MovieGenreFlags ComposeGenreFlags(IEnumerable<MovieGenre> genres)
    {
        var result = MovieGenreFlags.None;
        
        foreach (var genre in genres)
        {
            result |= GetFlagsGenre(genre);
        }

        return result;
    }
    
    public static void ExtractIndividualGenres(MovieGenreFlags genreFlags, ICollection<MovieGenre> outGenres)
    {
        var intGenre = (int)genreFlags;

        // Iterate the bits
        for (uint bit = 1; bit <= (uint)genreFlags; bit <<= 1)
        {
            // Is the bit set?
            if ((intGenre & bit) != 0)
            {
                // If we encounter a bad genre, continue
                var singleGenre = GetSingleGenre((MovieGenreFlags)bit);
                if (singleGenre == null)
                {
                    continue;
                }

                outGenres.Add(singleGenre.Value);
            }
        }
    }

    public static MovieGenreFlags GetFlagsGenre(MovieGenre genre)
    {
        return genre switch
        {
            MovieGenre.Action => MovieGenreFlags.Action,
            MovieGenre.Adventure => MovieGenreFlags.Adventure,
            MovieGenre.ScienceFiction => MovieGenreFlags.ScienceFiction,
            MovieGenre.Crime => MovieGenreFlags.Crime,
            MovieGenre.Mystery => MovieGenreFlags.Mystery,
            MovieGenre.Thriller => MovieGenreFlags.Thriller,
            MovieGenre.Animation => MovieGenreFlags.Animation,
            MovieGenre.Comedy => MovieGenreFlags.Comedy,
            MovieGenre.Family => MovieGenreFlags.Family,
            MovieGenre.Fantasy => MovieGenreFlags.Fantasy,
            MovieGenre.War => MovieGenreFlags.War,
            MovieGenre.Horror => MovieGenreFlags.Horror,
            MovieGenre.Drama => MovieGenreFlags.Drama,
            MovieGenre.Music => MovieGenreFlags.Music,
            MovieGenre.Romance => MovieGenreFlags.Romance,
            MovieGenre.Western => MovieGenreFlags.Western,
            MovieGenre.History => MovieGenreFlags.History,
            MovieGenre.TvMovie => MovieGenreFlags.TvMovie,
            MovieGenre.Documentary => MovieGenreFlags.Documentary,
            _ => throw new ArgumentException("Bad genre", nameof(genre))
        };
    }
    
    private static MovieGenre? GetSingleGenre(MovieGenreFlags genreFlag)
    {
        return genreFlag switch
        {
            MovieGenreFlags.Action => MovieGenre.Action,
            MovieGenreFlags.Adventure => MovieGenre.Adventure,
            MovieGenreFlags.ScienceFiction => MovieGenre.ScienceFiction,
            MovieGenreFlags.Crime => MovieGenre.Crime,
            MovieGenreFlags.Mystery => MovieGenre.Mystery,
            MovieGenreFlags.Thriller => MovieGenre.Thriller,
            MovieGenreFlags.Animation => MovieGenre.Animation,
            MovieGenreFlags.Comedy => MovieGenre.Comedy,
            MovieGenreFlags.Family => MovieGenre.Family,
            MovieGenreFlags.Fantasy => MovieGenre.Fantasy,
            MovieGenreFlags.War => MovieGenre.War,
            MovieGenreFlags.Horror => MovieGenre.Horror,
            MovieGenreFlags.Drama => MovieGenre.Drama,
            MovieGenreFlags.Music => MovieGenre.Music,
            MovieGenreFlags.Romance => MovieGenre.Romance,
            MovieGenreFlags.Western => MovieGenre.Western,
            MovieGenreFlags.History => MovieGenre.History,
            MovieGenreFlags.TvMovie => MovieGenre.TvMovie,
            MovieGenreFlags.Documentary => MovieGenre.Documentary,
            _ => null
        };
    }
}