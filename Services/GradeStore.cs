using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StudentGradeManager.Models;

namespace StudentGradeManager.Services;

public class GradeStore(GradeDbContext dbContext)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<GradeEntry>> GetAllAsync(string ownerId)
    {
        return await dbContext.GradeEntries
            .AsNoTracking()
            .Where(entry => entry.OwnerId == ownerId)
            .ToListAsync();
    }

    public Task<GradeEntry?> GetByIdAsync(int id, string ownerId)
    {
        return dbContext.GradeEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(entry => entry.Id == id && entry.OwnerId == ownerId);
    }

    public async Task AddAsync(GradeEntry entry, string ownerId)
    {
        entry.OwnerId = ownerId;
        dbContext.GradeEntries.Add(entry);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(GradeEntry entry, string ownerId)
    {
        var existing = await dbContext.GradeEntries
            .FirstOrDefaultAsync(item => item.Id == entry.Id && item.OwnerId == ownerId);
        if (existing is null)
        {
            return false;
        }

        existing.StudentName = entry.StudentName;
        existing.StudentId = entry.StudentId;
        existing.Course = entry.Course;
        existing.Score = entry.Score;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string ownerId)
    {
        var entry = await dbContext.GradeEntries
            .FirstOrDefaultAsync(item => item.Id == id && item.OwnerId == ownerId);
        if (entry is null)
        {
            return false;
        }

        dbContext.GradeEntries.Remove(entry);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task ClaimLegacyEntriesAsync(string ownerId)
    {
        await dbContext.GradeEntries
            .Where(entry => entry.OwnerId == null)
            .ExecuteUpdateAsync(update => update.SetProperty(entry => entry.OwnerId, ownerId));
    }

    public static async Task InitializeAsync(GradeDbContext dbContext, IWebHostEnvironment environment)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.LegacyDataImports.AnyAsync())
        {
            return;
        }

        var legacyPath = Path.Combine(environment.ContentRootPath, "App_Data", "grades.json");
        if (File.Exists(legacyPath))
        {
            var json = await File.ReadAllTextAsync(legacyPath);
            var entries = JsonSerializer.Deserialize<List<GradeEntry>>(json, JsonOptions)
                ?? throw new InvalidDataException($"Grade data in '{legacyPath}' is empty or invalid.");

            foreach (var entry in entries)
            {
                entry.OwnerId = null;
                dbContext.GradeEntries.Add(entry);
            }
        }

        dbContext.LegacyDataImports.Add(new LegacyDataImport { Id = 1 });
        await dbContext.SaveChangesAsync();
    }
}
