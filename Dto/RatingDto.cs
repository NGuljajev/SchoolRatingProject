using System;
using System.ComponentModel.DataAnnotations;

public class RatingCreateDto
{
    [Required]
    public int SubjectId { get; set; }

    [Required]
    public int TeacherId { get; set; }

    [Range(1, 5)] // ✅ matches DB CHECK constraint
    public int Score { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}
