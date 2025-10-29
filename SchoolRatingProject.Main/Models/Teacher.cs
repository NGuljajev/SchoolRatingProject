using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolRating.Models
{
    [Table("teacher")]
    public class Teacher
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        [EmailAddress]
        [Column("Email")]
        public string? Email { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
