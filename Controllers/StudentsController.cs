using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGradeManager.Models;
using StudentGradeManager.Services;

namespace StudentGradeManager.Controllers;

[Authorize]
public class StudentsController(GradeStore gradeStore) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var allEntries = await gradeStore.GetAllAsync(GetUserId());
        var entries = string.IsNullOrWhiteSpace(search)
            ? allEntries
            : allEntries.Where(entry =>
                entry.StudentName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                entry.StudentId.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                entry.Course.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        var model = new GradeDashboardViewModel
        {
            SearchTerm = search ?? string.Empty,
            Entries = entries.OrderBy(entry => entry.StudentName).ThenBy(entry => entry.Course).ToList(),
            StudentCount = allEntries.Select(entry => entry.StudentId).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            CourseCount = allEntries.Select(entry => entry.Course).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            AverageScore = allEntries.Count == 0 ? 0 : allEntries.Average(entry => entry.Score),
            PassRate = allEntries.Count == 0 ? 0 : allEntries.Count(entry => entry.Score >= 60) * 100d / allEntries.Count
        };

        return View(model);
    }

    public IActionResult Create() => View(new GradeEntry());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GradeEntry entry)
    {
        if (!ModelState.IsValid)
        {
            return View(entry);
        }

        await gradeStore.AddAsync(entry, GetUserId());
        TempData["SuccessMessage"] = $"Grade for {entry.StudentName} was added.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entry = await gradeStore.GetByIdAsync(id, GetUserId());
        return entry is null ? NotFound() : View(entry);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GradeEntry entry)
    {
        if (id != entry.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(entry);
        }

        if (!await gradeStore.UpdateAsync(entry, GetUserId()))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = $"Grade for {entry.StudentName} was updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var ownerId = GetUserId();
        var entry = await gradeStore.GetByIdAsync(id, ownerId);
        if (entry is null || !await gradeStore.DeleteAsync(id, ownerId))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = $"Grade for {entry.StudentName} was deleted.";
        return RedirectToAction(nameof(Index));
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("The authenticated user has no identifier.");
    }
}
