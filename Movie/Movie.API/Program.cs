using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Movie.API.Utility;
using Movie.Database;

namespace Movie.API;

public class Program
{
    public static async Task Main(string[] args)
    {
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
        builder.Services.AddSwaggerGen(options =>
        {
            // Locate the XML documentation file
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

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
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie.API Api");
                s.RoutePrefix = "api/info";
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        await app.RunAsync();
    }
}