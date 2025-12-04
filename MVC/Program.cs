using Microsoft.EntityFrameworkCore;
using VicemMVCIdentity.MVC.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Connection string cho DbContext của MVC.Data
var mvcConnectionString = builder.Configuration.GetConnectionString("DefaultConnection"); 

builder.Services.AddDbContext<MVC.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(mvcConnectionString));

// Identity DbContext
builder.Services.AddDbContext<VicemMVCIdentity.MVC.Areas.Identity.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(mvcConnectionString));

builder.Services.AddDefaultIdentity<VicemMVCIdentity.MVC.Areas.Identity.Data.ApplicationUser>()
    .AddEntityFrameworkStores<VicemMVCIdentity.MVC.Areas.Identity.Data.ApplicationDbContext>();


var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // thêm
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // cần cho Identity

app.Run();
