using Donations.Extensions;
using Donation.Infrastructure;
using Donation.Domain.Entities;
using Donation.Infrastructure.Persistence;
using Donation.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// ==========================================
// 1. تسجيل طبقة الـ Infrastructure
// ==========================================
builder.Services.AddInfrastructure(builder.Configuration);

// ==========================================
// 2. تسجيل الـ Identity أولاً
// ==========================================
builder.Services.AddIdentity<User, Role>(options =>
{
    // إعدادات الباسورد أو غيرها إن وجدت
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddRoles<Role>();

// ==========================================
// 3. ضبط الـ Cookies لمنع التحويل وإرجاع 401 مباشرة للـ API
// ==========================================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

// ==========================================
// 4. تسجيل الـ JWT ثانياً ليكون هو الأساس والمعتمد للـ API
// ==========================================
builder.Services.AddJwtAuthentication(builder.Configuration);

// ==========================================
// 5. سياسات الصلاحيات + فرض الحماية العامة (FallbackPolicy)
// ==========================================
builder.Services.AddAuthorization(options =>
{
    // فرض الحماية على كل الـ Endpoints تلقائياً (تتطلب تسجيل دخول حصراً)
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // السياسات الخاصة بالصلاحيات (Permissions)
    foreach (var permission in Donation.Application.Constants.Permissions.AllPermissionsList)
    {
        options.AddPolicy(permission, policy =>
            policy.RequireClaim("Permission", permission));
    }
});

var app = builder.Build();

// ==========================================
// تشغيل الـ Seeder لإدخال الأدوار والآدمن تلقائياً
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var logger = services.GetRequiredService<ILogger<RbacDbSeeder>>();

        await RbacDbSeeder.SeedAsync(userManager, roleManager, context, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration/seeding.");
    }
}

// ==========================================
// Configure the HTTP request pipeline.
app.UseSwaggerDocumentation();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();