using Microsoft.EntityFrameworkCore;
using Scoring_Service.Models.Entities;

namespace Scoring_Service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Database Sets 
        public DbSet<ConditionEvaulationResult> EvaulationResults { get; set; }
        public DbSet<CustomerRequest> CustomerRequests { get; set; }
    }
}
