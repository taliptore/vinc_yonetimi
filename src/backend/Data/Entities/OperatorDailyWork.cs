namespace VincYonetim.Api.Data.Entities;

public class OperatorDailyWork
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int OperatorId { get; set; }
    public DateTime WorkDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal TotalHours { get; set; } // Hesaplanan
    public decimal OvertimeHours { get; set; }

    public Job Job { get; set; } = null!;
    public Operator Operator { get; set; } = null!;
}
