using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Data.Entities;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/announcements")]
[Authorize(Roles = "Admin")]
public class AnnouncementsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public AnnouncementsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Announcement>>> List(CancellationToken ct)
    {
        var list = await _db.Announcements.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.CreatedAt).ToListAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Announcement>> Get(int id, CancellationToken ct)
    {
        var item = await _db.Announcements.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Announcement>> Create([FromBody] Announcement model, CancellationToken ct)
    {
        model.Id = 0;
        model.CreatedAt = DateTime.UtcNow;
        _db.Announcements.Add(model);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] Announcement model, CancellationToken ct)
    {
        var item = await _db.Announcements.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item == null) return NotFound();
        item.Title = model.Title;
        item.Body = model.Body;
        item.IsActive = model.IsActive;
        item.DisplayOrder = model.DisplayOrder;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await _db.Announcements.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item == null) return NotFound();
        _db.Announcements.Remove(item);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
