using airlineRider.DAL;
using airlineRider.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace airlineRider.Test;

public class TestingWebApplicationFactory: WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var postgreSqlContainer = new PostgreSqlBuilder("postgis/postgis:18-3.6")
            .WithEnvironment("POSTGRES_DB","airlineRyder")
            .WithEnvironment("POSTGRES_USER","ryder")
            .WithEnvironment("POSTGRES_PASSWORD","NOPE")
            .WithNetworkAliases("post")
            .WithExposedPort(5432)
            .Build();
        postgreSqlContainer.StartAsync().GetAwaiter().GetResult();;

        var redisContainer = new RedisBuilder("redis:latest")
            .WithExposedPort(6379)
            .Build();
        redisContainer.StartAsync().GetAwaiter().GetResult();;

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<TypeContext>>();
            services.AddDbContextPool<TypeContext>(options =>
            {
                options.UseNpgsql($"Server=localhost;Port={postgreSqlContainer.GetMappedPublicPort(5432)};Database=airlineRyder;User Id=ryder;Password=NOPE;");
            });

            services.RemoveAll<IConnectionMultiplexer>();
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"localhost:{redisContainer.GetMappedPublicPort(6379)}"));

            services.AddScoped<AircraftTypeService>();
            services.AddScoped<AirportService>();
            services.AddScoped<CountryService>();
            services.AddScoped<UserService>();
            services.AddScoped<LobbyService>();

            var sp = services.BuildServiceProvider();


            using var scope = sp.CreateScope();
            var typeContext = scope.ServiceProvider.GetRequiredService<TypeContext>();

            typeContext.Database.EnsureCreated();


        });

        
    }
}