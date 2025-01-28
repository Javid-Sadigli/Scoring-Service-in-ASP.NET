using Microsoft.EntityFrameworkCore;

namespace Scoring_Service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Database Sets 

    }
}
