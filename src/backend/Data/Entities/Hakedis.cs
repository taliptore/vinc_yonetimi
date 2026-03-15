namespace VincYonetim.Api.Data.Entities;

public class Hakedis
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string Period { get; set; } = string.Empty; // örn. "2024-01"
    public int TotalDays { get; set; }
    public decimal DailyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AdvanceDeduction { get; set; }
    public decimal NetAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Job Job { get; set; } = null!;
}
