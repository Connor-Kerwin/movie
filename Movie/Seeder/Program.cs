using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MovieDatabase;
using MovieDatabase.Entities;

namespace Seeder;

class Program
{
    static async Task Main(string[] args)
    {
        // NOTE: To simplify the implementation, a separate 'seeder' app has been chosen. This app is responsible
        // for the DB related operations such as migration and seeding from the csv. This was helpful to avoid
        // coupling the API to DB flow. In a microservice environment, this approach is nicer to work with!
        
        var builder = Host.CreateApplicationBuilder();

        builder.Configuration.AddJsonFile(
            "appsettings.json",
            optional: false,
            reloadOnChange: true);

        var connectionString = builder.Configuration.GetConnectionString("MovieDatabase");

        // We cannot continue if the connection string isn't in our configuration
        if (connectionString == null)
        {
            throw new Exception("MovieDatabase connection string was missing from the configuration");
        }

        builder.Services.AddDbContext<MovieDbContext>(options =>
        {
            // We assume MySql is being used as the database
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                dbOpts => { dbOpts.EnableRetryOnFailure(); });
        });

        var app = builder.Build();

        // Short circuit and shut the app down if we're running design time (EF Migrations)
        if (EF.IsDesignTime)
        {
            return;
        }

        await SeedDatabase(app);
    }

    private static async Task SeedDatabase(IHost app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var context = scope.ServiceProvider.GetRequiredService<MovieDbContext>();

            logger.LogInformation("Migrating database...");
            
            // Auto run migrations
            await context.Database.MigrateAsync();

            // NOTE: We assume that if there are no movies in the database, we need to do a migration/seed op.

            if (!context.Movies.Any())
            {
                logger.LogInformation("Seeding database...");

                if (!File.Exists("movies.csv"))
                {
                    logger.LogError("movies.csv not found");
                    return;
                }
                
                var exported = 0;
                using (var reader = new StreamReader("movies.csv"))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        // NOTE: This seems to be required because some of the data seems to be bad?
                        MissingFieldFound = null
                    };

                    using (var csv = new CsvReader(reader, config))
                    {
                        var output = csv.GetRecords<MovieCsvModel>().ToList();

                        foreach (var csvModel in output)
                        {
                            var model = new MovieEntity()
                            {
                                Title = csvModel.Title,
                                OriginalLanguage = csvModel.OriginalLanguage,
                                ReleaseDate = csvModel.ReleaseDate,
                                VoteAverage = csvModel.VoteAverage,
                                VoteCount = csvModel.VoteCount,
                                Overview = csvModel.Overview,
                                PosterUrl = csvModel.PosterUrl,
                                Popularity = csvModel.Popularity,
                            };
                            
                            var genre = ParseGenre(csvModel.Genre);
                            model.Genres = genre;

                            // TODO: Here, if there is a movie genre that is none, what is the best course of action?
                            //  A nullable genre would maybe work, but it makes usage awkward. Maybe unknown genre?

                            if (genre == MovieGenreFlags.None)
                            {
                                logger.LogWarning(
                                    $"Failed to identify movie genre '{csvModel.Genre}' for {csvModel.Title}");
                            }

                            exported++;
                            context.Movies.Add(model);
                        }
                    }

                    await context.SaveChangesAsync();
                }
                
                logger.LogInformation($"Migrated {exported} records into the database");
            }
            else // Already seeded
            {
                var count = await context.Movies.CountAsync();
                logger.LogInformation($"Database seeding skipped, data was detected ({count} items)");
            }
        }
    }

    private static MovieGenreFlags ParseGenre(string genre)
    {
        var result = MovieGenreFlags.None;

        var split = genre.Split(',');
        foreach (var item in split)
        {
            // We want to clean up the genre, whitespace, casing, etc.
            var cleaned = item.Trim().ToLower();
            var itemGenre = StringToGenre(cleaned);

            // A none-genre here is unknown.
            // Ideally we should actually handle this case. Unknown genre?
            if (itemGenre != MovieGenreFlags.None)
            {
                result |= itemGenre;
            }
        }

        return result;
    }

    private static MovieGenreFlags StringToGenre(string genre)
    {
        return genre switch
        {
            "action" => MovieGenreFlags.Action,
            "animation" => MovieGenreFlags.Animation,
            "adventure" => MovieGenreFlags.Adventure,
            "science fiction" => MovieGenreFlags.ScienceFiction,
            "crime" => MovieGenreFlags.Crime,
            "mystery" => MovieGenreFlags.Mystery,
            "thriller" => MovieGenreFlags.Thriller,
            "comedy" => MovieGenreFlags.Comedy,
            "family" => MovieGenreFlags.Family,
            "fantasy" => MovieGenreFlags.Fantasy,
            "war" => MovieGenreFlags.War,
            "horror" => MovieGenreFlags.Horror,
            "drama" => MovieGenreFlags.Drama,
            "music" => MovieGenreFlags.Music,
            "romance" => MovieGenreFlags.Romance,
            "western" => MovieGenreFlags.Western,
            "history" => MovieGenreFlags.History,
            "tv movie" => MovieGenreFlags.TvMovie,
            "documentary" => MovieGenreFlags.Documentary,
            _ => MovieGenreFlags.None
        };
    }

    [Delimiter(",")]
    public class MovieCsvModel
    {
        [Name("Release_Date")] public DateTime ReleaseDate { get; set; }
        [Name("Title")] public string Title { get; set; }
        [Name("Overview")] public string Overview { get; set; }
        [Name("Popularity")] public float Popularity { get; set; }
        [Name("Vote_Count")] public int VoteCount { get; set; }
        [Name("Vote_Average")] public float VoteAverage { get; set; }
        [Name("Original_Language")] public string OriginalLanguage { get; set; }
        [Name("Genre")] public string Genre { get; set; }
        [Name("Poster_Url")] public string PosterUrl { get; set; }
    }
}