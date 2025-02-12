using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scoring_Service.Data;
using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;

namespace Scoring_Service.IntegrationTests.Controllers
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly IConfiguration configuration; 
        private readonly MsSqlContainer dbContainer;

        public CustomWebApplicationFactory()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())  
                .AddJsonFile("appsettings-test.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();

            dbContainer = new MsSqlBuilder()
                .WithImage(configuration["TestDbContainer:DockerImage"])
                .WithName(configuration["TestDbContainer:ContainerName"])
                .WithPassword(configuration["TestDbContainer:DbPassword"])
                .WithPortBinding(1433, true) 
                .Build();
        }

        public async Task InitializeAsync()
        {
            await dbContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await dbContainer.StopAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services
                    .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                
                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(dbContainer.GetConnectionString()));

                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.Migrate();
                }
            });
        }
        
    }
}
