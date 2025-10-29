using System;
using System.ComponentModel.DataAnnotations;

public class SubjectCreateDto
{
    [Required]
    [MaxLength(100)] // ✅ matches DB column size
    public string Name { get; set; } = null!;

    [MaxLength(20)] // ✅ matches DB column size
    public string Code { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}
