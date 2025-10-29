using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolRating.Models
{
    [Table("rating")]
    public class Rating
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("TeacherId")]
        public int TeacherId { get; set; }

        public Teacher Teacher { get; set; } = null!;

        [Required]
        [Column("SubjectId")]
        public int SubjectId { get; set; }

        public Subject Subject { get; set; } = null!;

        [Range(1, 5)] // ✅ matches CHECK constraint in DB
        [Column("Score")]
        public int Score { get; set; }

        [Column("Comment")]
        public string? Comment { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
