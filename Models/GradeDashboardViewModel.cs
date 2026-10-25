namespace StudentGradeManager.Models;

public class GradeDashboardViewModel
{
    public string SearchTerm { get; set; } = string.Empty;
    public IReadOnlyList<GradeEntry> Entries { get; set; } = [];
    public int StudentCount { get; set; }
    public int CourseCount { get; set; }
    public double AverageScore { get; set; }
    public double PassRate { get; set; }
}
