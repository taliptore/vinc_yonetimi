namespace VincYonetim.Api.Data.Entities;

public class Payment
{
    public int Id { get; set; }
    public int? HakedisId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Type { get; set; }

    public Hakedis? Hakedis { get; set; }
}
