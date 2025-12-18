using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MVC.Areas.Identity.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;
using MVC.Services; // EmailSender
using MVC.Models.Process;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext cho Employee
builder.Services.AddDbContext<MVC.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
// DbContext (Identity)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;

    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAuthorization();

// EmailSender
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))
    .SetApplicationName("YourAppName")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in Enum.GetValues(typeof(SystemPermissions)).Cast<SystemPermissions>())
    {
        options.AddPolicy(permission.ToString(), policy =>
        {
            policy.RequireClaim("Permission", permission.ToString());
        });
    }
    //options.AddPolicy("Role", policy => 
    //    policy.RequireClaim("Role", "AdminOnly"));

    //options.AddPolicy("Permission", policy => 
     //   policy.RequireClaim("Role", "EmployeeOnly"));
    //options.AddPolicy("AccountView", policy =>
    //    policy.RequireClaim("Permission", "AccountView"));

    //options.AddPolicy("PolicyEmployee", policy =>
    //{
    //    policy.RequireRole("Employee");
    //});

    //options.AddPolicy("PolicyAdmin", policy =>
    //{
     //   policy.RequireRole("Admin");
    //});
    //options.AddPolicy("PolicyByPhoneNumber", policy =>
      //  policy.RequireAssertion(context =>
      //  {
      //      var phoneClaim = context.User.FindFirst(c => c.Type == "PhoneNumber");
       //     return phoneClaim != null && phoneClaim.Value == "0123456789";
       // }));
    
});
// Đăng ký Handler xử lý logic tùy chỉnh
builder.Services.AddSingleton<IAuthorizationHandler, PolicyByPhoneNumberHandler>();
// EmployeeSeeder
builder.Services.AddTransient<EmployeeSeeder>();

var app = builder.Build();

// Seed dữ liệu
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<EmployeeSeeder>();
    seeder.SeedEmployees(1000);
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
