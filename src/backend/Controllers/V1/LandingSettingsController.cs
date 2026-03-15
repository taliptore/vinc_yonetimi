using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Data.Entities;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/landing-settings")]
[Authorize(Roles = "Admin")]
public class LandingSettingsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public LandingSettingsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<LandingPageSetting>> Get(CancellationToken ct)
    {
        var s = await _db.LandingPageSettings.FirstOrDefaultAsync(ct);
        if (s == null)
        {
            s = new LandingPageSetting { Id = 1, UpdatedAt = DateTime.UtcNow };
            _db.LandingPageSettings.Add(s);
            await _db.SaveChangesAsync(ct);
        }
        return Ok(s);
    }

    [HttpPut]
    public async Task<ActionResult> Put([FromBody] LandingPageSetting model, CancellationToken ct)
    {
        var s = await _db.LandingPageSettings.FirstOrDefaultAsync(ct);
        if (s == null)
        {
            s = new LandingPageSetting { Id = 1 };
            _db.LandingPageSettings.Add(s);
            await _db.SaveChangesAsync(ct);
        }
        s.HeroTitle = model.HeroTitle ?? s.HeroTitle;
        s.HeroDescription = model.HeroDescription ?? s.HeroDescription;
        s.SliderImagesJson = model.SliderImagesJson;
        s.FeaturesJson = model.FeaturesJson;
        s.BenefitsJson = model.BenefitsJson;
        s.GalleryImagesJson = model.GalleryImagesJson;
        s.ContactEmail = model.ContactEmail;
        s.FooterText = model.FooterText;
        s.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
