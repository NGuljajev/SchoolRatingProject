using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class TeacherCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    // ✅ Avoid cycles by using DTOs instead of full Rating entities
    public List<RatingCreateDto>? Ratings { get; set; }
}
