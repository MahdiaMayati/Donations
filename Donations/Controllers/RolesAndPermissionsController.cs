using Donation.Domain.Entities; // تأكدي من تطابق الـ Namespace لديكِ
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Donation.Infrastructure.Persistence;
namespace Donation.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin")] // يفضل تفعيلها لاحقاً لحماية الـ Endpoints
public class RolesAndPermissionsController : ControllerBase
{
    private readonly RoleManager<Role> _roleManager; // استخدام كلاس Role الخاص بكِ
    private readonly UserManager<User> _userManager;

    public RolesAndPermissionsController(
        RoleManager<Role> roleManager,
        UserManager<User> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // ==========================================
    // 1. إدارة الأدوار (Roles: Create, Update, Delete, Get All)
    // ==========================================

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToListAsync();
        return Ok(roles);
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName))
            return BadRequest("اسم الدور مطلوب.");

        var roleExist = await _roleManager.RoleExistsAsync(request.RoleName);
        if (roleExist)
            return BadRequest("هذا الدور موجود مسبقاً.");

        var result = await _roleManager.CreateAsync(new Role { Name = request.RoleName });
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "تم إنشاء الدور بنجاح." });
    }

    [HttpPut("roles/{id}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) return NotFound("الدور غير موجود.");

        role.Name = request.RoleName;
        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "تم تحديث الدور بنجاح." });
    }

    [HttpDelete("roles/{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) return NotFound("الدور غير موجود.");

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "تم حذف الدور بنجاح." });
    }

    // ==========================================
    // 2. إدارة الصلاحيات (Permissions & Assignment)
    // ==========================================

    // جلب كل الصلاحيات الثابتة في النظام
    [HttpGet("permissions")]
    public IActionResult GetAllPermissions()
    {
        var allPermissions = Donation.Application.Constants.Permissions.AllPermissionsList;
        return Ok(allPermissions);
    }

    // جلب الصلاحيات الخاصة بدور معين
    [HttpGet("roles/{roleId}/permissions")]
    public async Task<IActionResult> GetRolePermissions(Guid roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null) return NotFound("الدور غير موجود.");

        var claims = await _roleManager.GetClaimsAsync(role);
        var permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();

        return Ok(permissions);
    }

    // تعيين أو تعديل صلاحيات دور معين
    [HttpPost("roles/assign-permissions")]
    public async Task<IActionResult> AssignPermissionsToRole([FromBody] UpdateRolePermissionsRequest request)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId);
        if (role == null) return NotFound("الدور غير موجود.");

        // جلب الصلاحيات الحالية الحالية للدور
        var currentClaims = await _roleManager.GetClaimsAsync(role);

        // حذف الصلاحيات القديمة من نوع Permission
        foreach (var claim in currentClaims.Where(c => c.Type == "Permission"))
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }

        // إضافة الصلاحيات الجديدة المرسلة
        foreach (var permission in request.Permissions)
        {
            await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Permission", permission));
        }

        return Ok(new { message = "تم تحديث صلاحيات الدور بنجاح." });
    }



    [HttpPost("users/assign-role")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleRequest request, [FromServices] AppDbContext context)
    {
        // البحث الشامل والمباشر في قاعدة البيانات لتجنب مشاكل الـ Normalized المفقودة
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName == request.Email);

        if (user == null)
            return NotFound("المستخدم غير موجود.");

        var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
        if (!roleExists)
            return NotFound("الدور غير موجود.");

        if (await _userManager.IsInRoleAsync(user, request.RoleName))
            return BadRequest("المستخدم يمتلك هذا الدور بالفعل.");

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = $"تم إسناد الدور '{request.RoleName}' للمستخدم بنجاح." });
    }
}




// ==========================================
// DTOs الخاصة بالـ Controller
// ==========================================
public class CreateRoleRequest
{
    public string RoleName { get; set; } = string.Empty;
}

public class UpdateRoleRequest
{
    public string RoleName { get; set; } = string.Empty;
}

public class UpdateRolePermissionsRequest
{
    public string RoleId { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}
public class AssignRoleRequest
{
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}