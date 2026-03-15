namespace VincYonetim.Api.Data.Entities;

public class JobPhoto
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public int? UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Job Job { get; set; } = null!;
}
