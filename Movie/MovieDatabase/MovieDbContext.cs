using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MovieDatabase;

public class MovieDbContext : DbContext
{
    public DbSet<MovieEntity> Movies { get; set; }

    public MovieDbContext(DbContextOptions<MovieDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<MovieEntity>()
            .ToTable("movies")
            .HasKey(m => m.Id);

        // Define movie genre enum/int conversion mapping
        modelBuilder.Entity<MovieEntity>()
            .Property(m => m.Genre)
            .HasConversion(new EnumToNumberConverter<MovieGenres, int>());
    }
}