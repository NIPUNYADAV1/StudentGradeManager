using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudentGradeManager.Models;

public class GradeDbContext(DbContextOptions<GradeDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<GradeEntry> GradeEntries => Set<GradeEntry>();
    public DbSet<LegacyDataImport> LegacyDataImports => Set<LegacyDataImport>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<GradeEntry>().HasIndex(entry => entry.OwnerId);
    }
}

public class LegacyDataImport
{
    public int Id { get; set; }
}
