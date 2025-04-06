using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDatabase;

[Table("mymoviedb")] // TODO: Dont need this
public class MovieModel
{
    // TODO: What wants to be nullable?
    
    public int Id { get; set; }
    public MovieGenres Genre { get; set; }
    public string? Title { get; set; }
    public string? Overview { get; set; }
    public float? Popularity { get; set; }
    public string? PosterUrl { get; set; }
    public int? VoteCount { get; set; }
    public double? VoteAverage { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? OriginalLanguage { get; set; }
}