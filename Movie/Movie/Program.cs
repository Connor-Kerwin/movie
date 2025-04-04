using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Movie.Db;
using MySqlConnector;

namespace Movie;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // TODO: Inject connection string / parameters from configuration!
        // URL is not working, hmm

        var sqlBuilder = new MySqlConnectionStringBuilder();
        sqlBuilder.Port = 3306;
        sqlBuilder.Server = "localhost";
        sqlBuilder.Database = "myDatabase";
        sqlBuilder.UserID = "root";
        sqlBuilder.Password = "yourpassword";
        var path = sqlBuilder.ToString();
        
        
            //jdbc:mysql://localhost:3306/mydatabase
            //Server=localhost;Database=mydatabase;User=root;Password=yourpassword;
        builder.Services.AddDbContext<MovieDbContext>(options =>
        {
            options.UseMySql(
                "Server=db;Database=mydatabase;User=root;Password=yourpassword;", 
                new MySqlServerVersion(new Version(8, 0, 28)));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}