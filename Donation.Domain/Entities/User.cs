using Microsoft.AspNetCore.Identity;

namespace Donation.Domain.Entities;

public class User : IdentityUser<Guid>
{
    // الحقول الإضافية الخاصة بكِ
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    /// <summary>Optional date of birth.</summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>true = Male, false = Female.</summary>
    public bool Gender { get; set; }

    public string PreferredContactMethod { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string EducationalStatus { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public string HealthStatus { get; set; } = string.Empty;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    // ملاحظة: الـ IdentityUser يعطينا مسبقاً Id, Email, PasswordHash, UserName, إلخ.
    // لذلك قومي بحذف التكرار إن وجد (مثل Id أو Email لو كانتا معرفتين مسبقاً بشكل يدوي).
}
