using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie.Db;

public class MovieDbContext : DbContext
{
    public DbSet<MovieModel> Movies { get; set; }

    public MovieDbContext(DbContextOptions<MovieDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MovieModel>()
            .ToTable("mymoviedb")
            .HasNoKey();
            //.HasKey(m => m.Id);
        //
        // modelBuilder.Entity<MovieModel>(entity =>
        // {
        //     entity.HasKey(m => m.Id);
        //     //entity.ToTable("mymoviedb");
        // });
    }
}

[Table("mymoviedb")]
public class MovieModel
{
  //  [Column("Id")] public int Id { get; set; }
    [Column("Genre")] public string? Genre { get; set; }
    [Column("Title")] public string? Title { get; set; }
    [Column("Overview")] public string? Overview { get; set; }
    [Column("Popularity")] public float? Popularity { get; set; }
    [Column("Poster_Url")] public string? PosterUrl { get; set; }
    [Column("Vote_Count")] public int? VoteCount { get; set; }
    [Column("Vote_Average")] public double? VoteAverage { get; set; }
    [Column("Release_Date")] public DateTime? ReleaseDate { get; set; }
    [Column("Original_Language")] public string? OriginalLanguage { get; set; }
}