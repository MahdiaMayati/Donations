using Donations.Extensions;
using Donation.Infrastructure;
using Donation.Domain.Entities;
using Donation.Infrastructure.Persistence;
using Donation.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Authentication & Swagger Extensions
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

// ==========================================
// 1. أولاً: تسجيل طبقة الـ Infrastructure (لتسجيل AppDbContext وقاعدة البيانات أولاً)
// ==========================================
builder.Services.AddInfrastructure(builder.Configuration);

// ==========================================
// 2. ثانياً: تسجيل الـ Identity وربطه بالـ AppDbContext (بعد أن أصبح مسجلاً في الـ DI)
// ==========================================
builder.Services.AddIdentity<User, Role>(options =>
{
    // إعدادات الباسورد أو غيرها إن وجدت
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddRoles<Role>();

// 3. سياسات الصلاحيات (Authorization Policies)
builder.Services.AddAuthorization(options =>
{
    foreach (var permission in Donation.Application.Constants.Permissions.AllPermissionsList)
    {
        options.AddPolicy(permission, policy =>
            policy.RequireClaim("Permission", permission));
    }
});

var app = builder.Build();

// ==========================================
// تشغيل الـ Seeder هنا لإدخال الأدوار والآدمن تلقائياً
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
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();