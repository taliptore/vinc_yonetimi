namespace VincYonetim.Api.Data.Entities;

public class SiteManager
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public Site Site { get; set; } = null!;
}
