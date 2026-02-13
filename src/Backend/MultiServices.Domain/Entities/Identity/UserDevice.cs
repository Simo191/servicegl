using MultiServices.Domain.Entities.Common;

namespace MultiServices.Domain.Entities.Identity;

public class UserDevice : BaseEntity
{
    public Guid UserId { get; set; }
    public string DeviceToken { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty; // iOS, Android
    public string? DeviceModel { get; set; }
    public string? AppVersion { get; set; }
    public bool IsActive { get; set; } = true;

    public ApplicationUser? User { get; set; }
}
