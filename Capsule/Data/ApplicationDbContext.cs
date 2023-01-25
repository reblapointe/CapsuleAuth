using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Capsule.Models;

namespace Capsule.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Capsule.Models.Vetement> Vetement { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}