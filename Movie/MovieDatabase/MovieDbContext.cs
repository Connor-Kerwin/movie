using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MovieDatabase;

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
            .ToTable("movies")
            .HasKey(m => m.Id);

        // Define movie genre enum/int conversion mapping
        modelBuilder.Entity<MovieModel>()
            .Property(m => m.Genre)
            .HasConversion(new EnumToNumberConverter<MovieGenres, int>());
    }
}