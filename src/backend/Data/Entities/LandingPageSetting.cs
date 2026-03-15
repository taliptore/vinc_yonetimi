namespace VincYonetim.Api.Data.Entities;

public class LandingPageSetting
{
    public int Id { get; set; }
    public string HeroTitle { get; set; } = "Vinç Yönetim Sistemi ile tüm operasyonlarınızı tek panelden yönetin";
    public string HeroDescription { get; set; } = "Vinç kiralama, operatör yönetimi, hakediş, şantiye takibi ve mobil saha uygulaması ile tüm süreçleri dijital yönetin.";
    public string? SliderImagesJson { get; set; } // JSON array of URLs
    public string? FeaturesJson { get; set; }   // JSON array of { title, description }
    public string? BenefitsJson { get; set; }   // JSON array of strings
    public string? GalleryImagesJson { get; set; }
    public string? ContactEmail { get; set; }
    public string? FooterText { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
