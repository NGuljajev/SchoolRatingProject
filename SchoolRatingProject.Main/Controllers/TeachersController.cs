using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolRating.Data;
using SchoolRating.Models;

[ApiController]
[Route("api/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly SchoolRatingDbContext _db;
    public TeachersController(SchoolRatingDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Teacher>>> Get() =>
        Ok(await _db.Teachers.Include(t => t.Ratings).AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Teacher>> Get(int id)
    {
        var t = await _db.Teachers.Include(x => x.Ratings).ThenInclude(r => r.Subject).FirstOrDefaultAsync(x => x.Id == id);
        if (t == null) return NotFound();
        return Ok(t);
    }

    [HttpPost]
    public async Task<ActionResult<Teacher>> Create([FromBody] TeacherCreateDto dto)
    {
        if (dto == null) return BadRequest("Body is required");

        var teacher = new Teacher
        {
            Name = dto.Name,
            Email = dto.Email,
            CreatedAt = dto.CreatedAt ?? DateTime.UtcNow,
            Ratings = new List<Rating>()
        };

        if (dto.Ratings != null && dto.Ratings.Any())
        {
            // Validate referenced subjects exist
            var subjectIds = dto.Ratings.Select(r => r.SubjectId).Distinct().ToList();
            var missingSubjects = await _db.Subjects.Where(s => subjectIds.Contains(s.Id)).Select(s => s.Id).ToListAsync();
            if (missingSubjects.Count != subjectIds.Count) return BadRequest("One or more Subjects referenced in ratings do not exist.");

            teacher.Ratings = dto.Ratings.Select(rdto => new Rating
            {
                SubjectId = rdto.SubjectId,
                Score = rdto.Score,
                Comment = rdto.Comment,
                CreatedAt = rdto.CreatedAt ?? DateTime.UtcNow
            }).ToList();
        }

        _db.Teachers.Add(teacher);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = teacher.Id }, teacher);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TeacherCreateDto dto)
    {
        var existing = await _db.Teachers.Include(t => t.Ratings).FirstOrDefaultAsync(t => t.Id == id);
        if (existing == null) return NotFound();

        existing.Name = dto.Name;
        existing.Email = dto.Email;
        existing.CreatedAt = dto.CreatedAt ?? existing.CreatedAt;

        // Replace ratings if provided
        if (dto.Ratings != null)
        {
            // Validate subject ids
            var subjectIds = dto.Ratings.Select(r => r.SubjectId).Distinct().ToList();
            var validSubjects = await _db.Subjects.Where(s => subjectIds.Contains(s.Id)).Select(s => s.Id).ToListAsync();
            if (validSubjects.Count != subjectIds.Count) return BadRequest("One or more Subjects referenced in ratings do not exist.");

            // Remove old ratings and add new mapped ones
            _db.Ratings.RemoveRange(existing.Ratings);
            existing.Ratings = dto.Ratings.Select(rdto => new Rating
            {
                SubjectId = rdto.SubjectId,
                Score = rdto.Score,
                Comment = rdto.Comment,
                CreatedAt = rdto.CreatedAt ?? DateTime.UtcNow
            }).ToList();
        }

        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { if (!await _db.Teachers.AnyAsync(x => x.Id == id)) return NotFound(); else throw; }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Teachers.FindAsync(id);
        if (t == null) return NotFound();
        _db.Teachers.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
