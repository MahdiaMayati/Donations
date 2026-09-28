using Donations.Extensions;
using Donation.Application;
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
// 1. تسجيل طبقة الـ Application (CQRS / MediatR)
// ==========================================
builder.Services.AddApplication();

// ==========================================
// 2. تسجيل طبقة الـ Infrastructure (AppDbContext وقاعدة البيانات)
// ==========================================
builder.Services.AddInfrastructure(builder.Configuration);

// ==========================================
// 3. تسجيل الـ Identity وربطه بالـ AppDbContext
// ==========================================
builder.Services.AddIdentity<User, Role>(options =>
{
    // إعدادات الباسورد أو غيرها إن وجدت
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddRoles<Role>();

// ==========================================
// 4. JWT AFTER Identity + cookie redirect disabled (see AuthenticationExtensions).
//    Without this, unauthorized/forbidden API calls redirect to /Account/Login → fake 404.
// ==========================================
builder.Services.AddJwtAuthentication(builder.Configuration);

// Ensure JWT remains the default schemes after Identity cookie registration.
builder.Services.PostConfigure<Microsoft.AspNetCore.Authentication.AuthenticationOptions>(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
});

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
