using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using Movie.Db;

namespace Movie;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // NOTE: The database (MySQL) is deployed as part of the compose step.
        // In production, this would not be the best idea. It should be hosted separately.
        // For ease of use, it is coupled.
        
        // NOTE: The connection string has been embedded into the appsettings.json file.
        // This is not an ideal scenario because we end up versioning the credentials.
        // Instead, we should use user secrets or some other kind of deployment steps
        // to inject the credentials. For ease of use though, we just include it anyway.
        
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
                new MySqlServerVersion(new Version(8, 0, 28)));
        });

        var app = builder.Build();

        await SeedDatabase(app);
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Api");
                s.RoutePrefix = "api/info";
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    private static async Task SeedDatabase(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
            await context.Database.MigrateAsync();

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
                }
            }
            
            var file = File.Exists("movies.csv");
            Console.WriteLine(file);
            
            // Not the best, but it should work fine for the requirements of the app
            if (!context.Movies.Any())
            {
                // NOTE: We expect that movies.csv gets copied into the container.
                // Ideally we'd expect the database to already be seeded with data
                // from another source/app, but this will work just fine.
                
                // TODO: Inject data from the csv
                
                Console.WriteLine("SEEDING DATABASE");
            }
        }
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