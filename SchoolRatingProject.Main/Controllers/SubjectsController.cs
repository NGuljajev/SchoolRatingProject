using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolRating.Data;
using SchoolRating.Models;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly SchoolRatingDbContext _db;
    public SubjectsController(SchoolRatingDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Subject>>> Get() =>
        Ok(await _db.Subjects.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Subject>> Get(int id)
    {
        var s = await _db.Subjects.FindAsync(id);
        if (s == null) return NotFound();
        return Ok(s);
    }

    [HttpPost]
    public async Task<ActionResult<Subject>> Create([FromBody] SubjectCreateDto dto)
    {
        if (dto == null) return BadRequest("Body is required");

        var subject = new Subject
        {
            Name = dto.Name,
            Code = dto.Code,
            CreatedAt = dto.CreatedAt ?? DateTime.UtcNow
        };

        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = subject.Id }, subject);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SubjectCreateDto dto)
    {
        var existing = await _db.Subjects.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = dto.Name;
        existing.Code = dto.Code;
        existing.CreatedAt = dto.CreatedAt ?? existing.CreatedAt;

        _db.Entry(existing).State = EntityState.Modified;
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { if (!await _db.Subjects.AnyAsync(x => x.Id == id)) return NotFound(); else throw; }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await _db.Subjects.FindAsync(id);
        if (s == null) return NotFound();
        _db.Subjects.Remove(s);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
