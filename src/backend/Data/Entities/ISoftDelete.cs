namespace VincYonetim.Api.Data.Entities;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    int? DeletedBy { get; set; }
}
