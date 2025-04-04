using Microsoft.EntityFrameworkCore;
using Movie.Db;

namespace Movie;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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