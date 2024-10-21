using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WDP2024Assignment2.Models;

namespace WDP2024Assignment2.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

public DbSet<WDP2024Assignment2.Models.AIImage> AIImage { get; set; } = default!;
}
