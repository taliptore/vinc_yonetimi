using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/public")]
[AllowAnonymous]
public class PublicController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PublicController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet("landing-settings")]
    public async Task<ActionResult<object>> GetLandingSettings(CancellationToken ct)
    {
        var s = await _db.LandingPageSettings.AsNoTracking().FirstOrDefaultAsync(ct);
        if (s == null)
            return Ok(new
            {
                heroTitle = "Vinç Yönetim Sistemi ile tüm operasyonlarınızı tek panelden yönetin",
                heroDescription = "Vinç kiralama, operatör yönetimi, hakediş, şantiye takibi ve mobil saha uygulaması ile tüm süreçleri dijital yönetin.",
                sliderImages = Array.Empty<string>(),
                features = new[] { new { title = "Vinç Yönetimi", description = "Vinçlerinizi plaka, tonaj ve kullanım durumuna göre takip edin." }, new { title = "Operatör Yönetimi", description = "Operatörlerin çalışma saatleri ve performansını izleyin." }, new { title = "Şantiye Yönetimi", description = "Kiralanan vinçlerin hangi şantiyede çalıştığını görün." } },
                benefits = new[] { "vinç doluluk analizi", "operatör yevmiye takibi", "hakediş hesaplama", "mobil saha yönetimi", "yakıt ve bakım takibi" },
                galleryImages = Array.Empty<string>(),
                contactEmail = "",
                footerText = ""
            });
        var slider = string.IsNullOrEmpty(s.SliderImagesJson) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(s.SliderImagesJson) ?? new List<string>();
        var gallery = string.IsNullOrEmpty(s.GalleryImagesJson) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(s.GalleryImagesJson) ?? new List<string>();
        var defaultFeatures = new[] { new { title = "Vinç Yönetimi", description = "Vinçlerinizi plaka, tonaj ve kullanım durumuna göre takip edin." }, new { title = "Operatör Yönetimi", description = "Operatörlerin çalışma saatleri ve performansını izleyin." }, new { title = "Şantiye Yönetimi", description = "Kiralanan vinçlerin hangi şantiyede çalıştığını görün." } };
        var defaultBenefits = new[] { "vinç doluluk analizi", "operatör yevmiye takibi", "hakediş hesaplama", "mobil saha yönetimi", "yakıt ve bakım takibi" };
        object[] featuresOut = defaultFeatures;
        if (!string.IsNullOrEmpty(s.FeaturesJson))
        {
            var parsed = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, string>>>(s.FeaturesJson);
            if (parsed != null && parsed.Count > 0)
                featuresOut = parsed.Select(x => new { title = x.GetValueOrDefault("title", ""), description = x.GetValueOrDefault("description", "") }).Cast<object>().ToArray();
        }
        var benefitsList = string.IsNullOrEmpty(s.BenefitsJson) ? defaultBenefits.ToList() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(s.BenefitsJson) ?? defaultBenefits.ToList();
        if (benefitsList.Count == 0) benefitsList = defaultBenefits.ToList();
        return Ok(new
        {
            heroTitle = s.HeroTitle,
            heroDescription = s.HeroDescription,
            sliderImages = slider,
            features = featuresOut,
            benefits = benefitsList,
            galleryImages = gallery,
            contactEmail = s.ContactEmail ?? "",
            footerText = s.FooterText ?? ""
        });
    }

    [HttpGet("announcements")]
    public async Task<ActionResult<List<object>>> GetAnnouncements(CancellationToken ct)
    {
        var list = await _db.Announcements.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new { x.Id, x.Title, x.Body, x.CreatedAt })
            .ToListAsync(ct);
        return Ok(list);
    }
}
