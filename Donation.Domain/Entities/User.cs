using Microsoft.AspNetCore.Identity;

namespace Donation.Domain.Entities;

public class User : IdentityUser<Guid>
{
    // الحقول الإضافية الخاصة بكِ
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    // ملاحظة: الـ IdentityUser يعطينا مسبقاً Id, Email, PasswordHash, UserName, إلخ.
    // لذلك قومي بحذف التكرار إن وجد (مثل Id أو Email لو كانتا معرفتين مسبقاً بشكل يدوي).
}