using MovieDatabase;

namespace Movie.Models;

public static class GenreMapper
{
    public static string GetGenreString(MovieGenre genre)
    {
        return genre.ToString().ToLowerInvariant();
    }

    public static bool ParseSingleGenre(string str, out MovieGenre genre)
    {
        return Enum.TryParse<MovieGenre>(str, true, out genre);
    }
    
    public static void ExtractGenres(MovieGenres genres, ICollection<MovieGenre> outGenres)
    {
        var intGenre = (int)genres;

        // Iterate the bits
        for (uint bit = 1; bit <= (uint)genres; bit <<= 1)
        {
            // Is the bit set?
            if ((intGenre & bit) != 0)
            {
                // If we encounter a bad genre, continue
                var singleGenre = GetSingleGenre((MovieGenres)bit);
                if (singleGenre == null)
                {
                    continue;
                }

                outGenres.Add(singleGenre.Value);
            }
        }
    }

    public static MovieGenres GetFlagsGenre(MovieGenre genre)
    {
        return genre switch
        {
            MovieGenre.Action => MovieGenres.Action,
            MovieGenre.Adventure => MovieGenres.Adventure,
            MovieGenre.ScienceFiction => MovieGenres.ScienceFiction,
            MovieGenre.Crime => MovieGenres.Crime,
            MovieGenre.Mystery => MovieGenres.Mystery,
            MovieGenre.Thriller => MovieGenres.Thriller,
            MovieGenre.Animation => MovieGenres.Animation,
            MovieGenre.Comedy => MovieGenres.Comedy,
            MovieGenre.Family => MovieGenres.Family,
            MovieGenre.Fantasy => MovieGenres.Fantasy,
            MovieGenre.War => MovieGenres.War,
            MovieGenre.Horror => MovieGenres.Horror,
            MovieGenre.Drama => MovieGenres.Drama,
            MovieGenre.Music => MovieGenres.Music,
            MovieGenre.Romance => MovieGenres.Romance,
            MovieGenre.Western => MovieGenres.Western,
            MovieGenre.History => MovieGenres.History,
            MovieGenre.TvMovie => MovieGenres.TvMovie,
            MovieGenre.Documentary => MovieGenres.Documentary,
            _ => throw new ArgumentException("Bad genre", nameof(genre))
        };
    }
    
    private static MovieGenre? GetSingleGenre(MovieGenres genre)
    {
        return genre switch
        {
            MovieGenres.Action => MovieGenre.Action,
            MovieGenres.Adventure => MovieGenre.Adventure,
            MovieGenres.ScienceFiction => MovieGenre.ScienceFiction,
            MovieGenres.Crime => MovieGenre.Crime,
            MovieGenres.Mystery => MovieGenre.Mystery,
            MovieGenres.Thriller => MovieGenre.Thriller,
            MovieGenres.Animation => MovieGenre.Animation,
            MovieGenres.Comedy => MovieGenre.Comedy,
            MovieGenres.Family => MovieGenre.Family,
            MovieGenres.Fantasy => MovieGenre.Fantasy,
            MovieGenres.War => MovieGenre.War,
            MovieGenres.Horror => MovieGenre.Horror,
            MovieGenres.Drama => MovieGenre.Drama,
            MovieGenres.Music => MovieGenre.Music,
            MovieGenres.Romance => MovieGenre.Romance,
            MovieGenres.Western => MovieGenre.Western,
            MovieGenres.History => MovieGenre.History,
            MovieGenres.TvMovie => MovieGenre.TvMovie,
            MovieGenres.Documentary => MovieGenre.Documentary,
            _ => null
        };
    }
}