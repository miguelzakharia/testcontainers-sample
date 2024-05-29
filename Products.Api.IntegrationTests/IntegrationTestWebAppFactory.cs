using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Api;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;

namespace Application.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
    //     .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
    //     .WithPassword("Strong_password_123!")
    //     .Build();

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
    	.WithImage("postgres")
    	// .WithImage("postgres:16")
    	.WithDatabase("apps_test")
    	.WithUsername("duck")
    	.WithPassword("duck")
    	// .WithPortBinding(5432, _port)
    	// .WithPortBinding(_port, 5432)
    	// .WithCleanUp(true)
    	// .WithLogger(logger)
    	// .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
    	.Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // services.AddDbContext<ApplicationDbContext>(options =>
            //     options.UseSqlServer(_dbContainer.GetConnectionString()));
            services.AddDbContext<ApplicationDbContext>(options =>
	            options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}
