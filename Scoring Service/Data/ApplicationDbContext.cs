using Microsoft.EntityFrameworkCore;
using Scoring_Service.Models.Entities;

namespace Scoring_Service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Database Sets 
        public virtual DbSet<ConditionEvaulationResult> EvaulationResults { get; set; }
        public virtual DbSet<CustomerRequest> CustomerRequests { get; set; }
    }
}
