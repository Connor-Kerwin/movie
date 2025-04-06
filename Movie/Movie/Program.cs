using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Movie.Controllers;
using Movie.Models;
using MovieDatabase;

namespace Movie;

public class Program
{
    private static void Test()
    {
        var entity = new MovieEntity();

        entity.Genre = MovieGenres.Comedy | MovieGenres.Horror | MovieGenres.Thriller | MovieGenres.Romance;
        //entity.Genre = (MovieGenres)int.MaxValue;

        var a = new MovieModel(entity);
    }

    public static async Task Main(string[] args)
    {
        Test();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options =>
        {
            // We want to handle some MySql exceptions to return better errors
            options.Filters.Add(new MySqlExceptionFilter());
        }).AddJsonOptions(opt =>
        {
            // NOTE: This has been done on purpose for all enum values.
            // For consistency between enums being used in data models and URL query parameters, it's been set to a fixed casing (kebab case feels nicest)
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower));
        });


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            // c.MapType<List<MovieGenre>>(() => new OpenApiSchema
            // {
            //     Type = "array",
            //     Items = new OpenApiSchema
            //     {
            //         Type = "string",
            //         Enum = Enum.GetNames(typeof(MovieGenre))
            //             .Select(name => new Microsoft.OpenApi.Any.OpenApiString(name))
            //             .Cast<Microsoft.OpenApi.Any.IOpenApiAny>()
            //             .ToList()
            //     }
            // });
            //
            //
            // c.SchemaFilter<SortModeSchemaFilter>();
            // c.SchemaFilter<MovieGenreSchemaFilter>();

            // TODO: Following this approach, the genre filter can be setup like this too!
            // TODO: This should actually be done as an ISchemaFilter instead!
            //
            // var enm = new List<IOpenApiAny>
            // {
            //     new OpenApiString(nameof(SortMode.Title).ToLower()),
            //     new OpenApiString(nameof(SortMode.ReleaseDate).ToLower())
            // };
            //
            // c.MapType<SortMode>(() => new OpenApiSchema
            // {
            //     Type = "string",
            //     Enum = enm
            // });
        });

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
            var version = new MySqlServerVersion(new Version(8, 0, 21));

            // We assume MySql is being used as the database
            options.UseMySql(
                connectionString,
                version);
            //dbOpts => { dbOpts.EnableRetryOnFailure(); });
        });

        var app = builder.Build();

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
}