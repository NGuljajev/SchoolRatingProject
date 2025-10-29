using Microsoft.EntityFrameworkCore;
using SchoolRating.Models;

namespace SchoolRating.Data
{
    public class SchoolRatingDbContext : DbContext
    {
        public SchoolRatingDbContext(DbContextOptions<SchoolRatingDbContext> options) : base(options) { }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Teacher>()
                .HasIndex(t => t.Email)
                .IsUnique();

            builder.Entity<Subject>()
                .HasIndex(s => s.Code)
                .IsUnique(false);

            builder.Entity<Rating>()
                .HasOne(r => r.Teacher)
                .WithMany(t => t.Ratings)
                .HasForeignKey(r => r.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Rating>()
                .HasOne(r => r.Subject)
                .WithMany(s => s.Ratings)
                .HasForeignKey(r => r.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
//test