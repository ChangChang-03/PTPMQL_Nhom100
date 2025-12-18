using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Areas.Identity.Data;
using MVC.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace MVC.Controllers
{
    [Authorize(Policy = "PolicyByPhoneNumber")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Policy = nameof(SystemPermissions.AccountView))]
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả người dùng từ UserManager
            var users = await _userManager.Users.ToListAsync();

            // Khởi tạo danh sách ViewModel
            var model = new List<UserWithRoleVM>();

            // Lấy roles của từng user
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserWithRoleVM
                {
                    User = user,
                    Roles = roles.ToList()
                });
            }

            // Trả về View kèm dữ liệu
            return View(model);
        }
        [Authorize(Policy = nameof(SystemPermissions.AssignRole))]
        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.Select(r => new RoleVM { Id = r.Id, Name = r.Name }).ToListAsync();
            
            var viewModel = new AssignRoleVM
            {
                UserId = userId,
                AllRoles = allRoles,
                SelectedRoles = userRoles
            };

            return View(viewModel);
        }
        public async Task<IActionResult> AddClaim(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var model = new UserClaimVM(userId, user.UserName, userClaims.ToList());
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                // Thêm các quyền mới được chọn mà người dùng chưa có
                foreach (var role in model.SelectedRoles)
                {
                    if (!userRoles.Contains(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }

                // Loại bỏ các quyền cũ không còn được chọn
                foreach (var role in userRoles)
                {
                    if (!model.SelectedRoles.Contains(role))
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }

                return RedirectToAction("Index", "Account");
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddClaim(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
            
            if (result.Succeeded)
            {
                return RedirectToAction("AddClaim", new { userId });
            }
            
            return View();
        }
    }
}