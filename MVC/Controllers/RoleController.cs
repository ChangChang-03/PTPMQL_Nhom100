using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models.ViewModels;
using System.Security.Claims;
using MVC.Models;
using MVC.Data;

namespace MVC.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RoleController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = new IdentityRole(roleName.Trim());
                await _roleManager.CreateAsync(role);
            }

            return RedirectToAction("Index");
        }

        // Edit role
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string newName)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            role.Name = newName;
            await _roleManager.UpdateAsync(role);
            return RedirectToAction("Index");
        }

        // Delete role
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }

        // Assign claim (GET)
        public async Task<IActionResult> AssignClaim(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return BadRequest();

            // Lấy tất cả permissions
            var allPermissions = Enum.GetValues(typeof(SystemPermissions))
                                     .Cast<SystemPermissions>()
                                     .Select(p => p.ToString())
                                     .ToList();

            var roleClaims = await _roleManager.GetClaimsAsync(role) ?? new List<Claim>();

            var model = new RoleClaimVM
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Claims = allPermissions.Select(p => new RoleClaim
                {
                    Type = "Permission",
                    Value = p,
                    Selected = roleClaims.Any(c => c.Type == "Permission" && c.Value == p)
                }).ToList()
            };

            return View(model);
        }

        // Assign claim (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignClaim(RoleClaimVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null) return BadRequest();

            // Sử dụng _context cho transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Xóa các claim cũ
                    var existingClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var claim in existingClaims.Where(c => c.Type == "Permission"))
                    {
                        await _roleManager.RemoveClaimAsync(role, claim);
                    }

                    // Thêm claim mới
                    var selectedClaims = model.Claims
                                              .Where(c => c.Selected)
                                              .Select(c => new Claim(c.Type, c.Value));

                    foreach (var claim in selectedClaims)
                    {
                        await _roleManager.AddClaimAsync(role, claim);
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "An error occurred while updating permissions.");
                    return View(model);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
