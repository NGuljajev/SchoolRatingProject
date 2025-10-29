using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolRating.Data;
using SchoolRating.Models;

[ApiController]
[Route("api/[controller]")]
public class RatingsController : ControllerBase
{
    private readonly SchoolRatingDbContext _db;
    public RatingsController(SchoolRatingDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rating>>> Get() =>
        Ok(await _db.Ratings.Include(r => r.Teacher).Include(r => r.Subject).AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Rating>> Get(int id)
    {
        var r = await _db.Ratings.Include(x => x.Teacher).Include(x => x.Subject).FirstOrDefaultAsync(x => x.Id == id);
        if (r == null) return NotFound();
        return Ok(r);
    }

    [HttpPost]
    public async Task<ActionResult<Rating>> Create([FromBody] RatingCreateDto dto)
    {
        if (dto == null) return BadRequest("Body is required");

        var teacherExists = await _db.Teachers.AnyAsync(t => t.Id == dto.TeacherId);
        var subjectExists = await _db.Subjects.AnyAsync(s => s.Id == dto.SubjectId);
        if (!teacherExists || !subjectExists) return BadRequest("Teacher or Subject not found.");

        var rating = new Rating
        {
            TeacherId = dto.TeacherId,
            SubjectId = dto.SubjectId,
            Score = dto.Score,
            Comment = dto.Comment,
            CreatedAt = dto.CreatedAt ?? DateTime.UtcNow
        };

        _db.Ratings.Add(rating);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = rating.Id }, rating);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RatingCreateDto dto)
    {
        var existing = await _db.Ratings.FindAsync(id);
        if (existing == null) return NotFound();

        if (!await _db.Teachers.AnyAsync(t => t.Id == dto.TeacherId) || !await _db.Subjects.AnyAsync(s => s.Id == dto.SubjectId))
            return BadRequest("Teacher or Subject not found.");

        existing.TeacherId = dto.TeacherId;
        existing.SubjectId = dto.SubjectId;
        existing.Score = dto.Score;
        existing.Comment = dto.Comment;
        existing.CreatedAt = dto.CreatedAt ?? existing.CreatedAt;

        _db.Entry(existing).State = EntityState.Modified;
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { if (!await _db.Ratings.AnyAsync(x => x.Id == id)) return NotFound(); else throw; }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _db.Ratings.FindAsync(id);
        if (r == null) return NotFound();
        _db.Ratings.Remove(r);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
