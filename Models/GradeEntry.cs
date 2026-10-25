using System.ComponentModel.DataAnnotations;

namespace StudentGradeManager.Models;

public class GradeEntry
{
    public int Id { get; set; }

    public string? OwnerId { get; set; }

    [Required]
    [StringLength(80)]
    [Display(Name = "Student name")]
    public string StudentName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Display(Name = "Student ID")]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Course { get; set; } = string.Empty;

    [Range(0, 100)]
    [Display(Name = "Score (%)")]
    public int Score { get; set; }

    public string LetterGrade => Score switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _ => "F"
    };
}
