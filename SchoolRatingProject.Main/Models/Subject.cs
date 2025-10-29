using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolRating.Models
{
    [Table("subject")]
    public class Subject
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)] // ✅ matches DB column size
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)] // ✅ matches DB column size
        [Column("Code")]
        public string? Code { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
