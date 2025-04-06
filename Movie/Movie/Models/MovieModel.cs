using MovieDatabase;

namespace Movie.Models;

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
    public List<string> Genres { get; } = [];

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

        ExtractGenres(entity.Genre);
    }

    private void ExtractGenres(MovieGenres genre)
    {
        // NOTE: In here, we iterate the bits of the enum.
        // If we encounter a bit that is set, we cast it to the enum and add it.
        
        var intGenre = (int)genre;
        
        // Iterate the bits
        for (uint bit = 1; bit <= (uint)genre; bit <<= 1)
        {
            // Is the bit set?
            if ((intGenre & bit) != 0)
            {
                // If we encounter an unknown genre, do nothing
                if (!Enum.IsDefined(typeof(MovieGenres), (int)bit))
                {
                    continue;
                }
                
                var rawStr = ((MovieGenres)bit).ToString();
                Genres.Add(rawStr);
            }
        }
    }
}