using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MovieDatabase;

namespace Seeder;

class Program
{
    static async Task Main(string[] args)
    {
        // TODO: This needs to initiate a database migration and seed the data.
        //  May want to do this as separate steps via args?

        var builder = Host.CreateApplicationBuilder();

        // TODO: Config? (create appsettings, etc.)
        var connectionString =
            "Server=host.docker.internal;Port=3306;Database=mydatabase;User=root;Password=yourpassword;";

        builder.Services.AddDbContext<MovieDbContext>(options =>
        {
            // We assume MySql is being used as the database
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                dbOpts => { dbOpts.EnableRetryOnFailure(); });
        });

        // TODO: Initialize services, etc. Probably dont need much, dbcontext, etc.

        var app = builder.Build();

        // Short circuit and shut the app down if we're running design time (EF Migrations)
        if (EF.IsDesignTime)
        {
            return;
        }

        await SeedDatabase(app);

        Console.WriteLine("Hello, World!");
    }

    private static async Task SeedDatabase(IHost app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<MovieDbContext>();

            // Auto run migrations
            await context.Database.MigrateAsync();

            // NOTE: We assume that if there are no movies in the database, we need to do a migration/seed op.

            if (!context.Movies.Any())
            {
                using (var reader = new StreamReader("movies.csv"))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        // NOTE: This seems to be required because some of the data seems to be bad?
                        MissingFieldFound = null
                    };

                    using (var csv = new CsvReader(reader, config))
                    {
                        var genres = new HashSet<string>();
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
                            model.Genre = genre;

                            // TODO: Here, if there is a movie genre that is none, what is the best course of action?
                            //  A nullable genre would maybe work, but it makes usage awkward. Maybe unknown genre?
                            
                            if (genre == MovieGenres.None)
                            {
                                Console.WriteLine($"Failed to identify movie genre '{csvModel.Genre}' for {csvModel.Title}");
                            }
                            
                            context.Movies.Add(model);

                            var csvGenres = csvModel.Genre.Split(',');
                            foreach (var g in csvGenres)
                            {
                                var clean = g.Trim().ToLower();
                                genres.Add(clean);
                            }
                        }
                    }
                    
                    await context.SaveChangesAsync();
                }
            }
        }
    }

    private static MovieGenres ParseGenre(string genre)
    {
        var result = MovieGenres.None;

        var split = genre.Split(',');
        foreach (var item in split)
        {
            // We want to clean up the genre, whitespace, casing, etc.
            var cleaned = item.Trim().ToLower();
            var itemGenre = StringToGenre(cleaned);

            // A none-genre here is unknown.
            // Ideally we should actually handle this case. Unknown genre?
            if (itemGenre != MovieGenres.None)
            {
                result |= itemGenre;
            }
        }
        
        return result;
    }

    private static MovieGenres StringToGenre(string genre)
    {
        return genre switch
        {
            "action" => MovieGenres.Action,
            "animation" => MovieGenres.Animation,
            "adventure" => MovieGenres.Adventure,
            "science fiction" => MovieGenres.ScienceFiction,
            "crime" => MovieGenres.Crime,
            "mystery" => MovieGenres.Mystery,
            "thriller" => MovieGenres.Thriller,
            "comedy" => MovieGenres.Comedy,
            "family" => MovieGenres.Family,
            "fantasy" => MovieGenres.Fantasy,
            "war" => MovieGenres.War,
            "horror" => MovieGenres.Horror,
            "drama" => MovieGenres.Drama,
            "music" => MovieGenres.Music,
            "romance" => MovieGenres.Romance,
            "western" => MovieGenres.Western,
            "history" => MovieGenres.History,
            "tv movie" => MovieGenres.TvMovie,
            "documentary" => MovieGenres.Documentary,
            _ => MovieGenres.None
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