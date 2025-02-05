using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scoring_Service.Data;
using Microsoft.Extensions.Configuration;

namespace Scoring_Service.IntegrationTests.Controllers
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly IConfiguration configuration;

        public CustomWebApplicationFactory()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())  
                .AddJsonFile("appsettings-test.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                String? dbConnectionString = configuration.GetConnectionString("TestConnection");

                services.AddDbContext<ApplicationDbContext>(options => options
                    .UseSqlServer(dbConnectionString));

                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.ExecuteSqlRaw("EXEC sp_MSforeachtable 'DROP TABLE IF EXISTS ?'");
                    dbContext.Database.Migrate();
                }
            });
        }
    }
}
