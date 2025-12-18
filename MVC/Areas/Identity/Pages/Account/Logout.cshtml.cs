// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.
#nullable disable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MVC.Areas.Identity.Data; //thêm namespace ApplicationUser

namespace MVC.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // Đăng xuất người dùng
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            // Nếu có returnUrl thì redirect về, không thì về trang chủ
            return returnUrl != null ? LocalRedirect(returnUrl) : RedirectToPage("/Index", new { area = "" });
        }
    }
}
