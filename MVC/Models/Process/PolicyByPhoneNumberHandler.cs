using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using MVC.Areas.Identity.Data; 

namespace MVC.Models.Process
{
    public class PolicyByPhoneNumberRequirement : IAuthorizationRequirement { }

    public class PolicyByPhoneNumberHandler : AuthorizationHandler<PolicyByPhoneNumberRequirement>
    {
        private readonly IServiceProvider _serviceProvider;

        public PolicyByPhoneNumberHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyByPhoneNumberRequirement requirement)
        {
            var httpContext = _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            if (httpContext == null) return;

            var userManager = httpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(context.User);

            // Kiểm tra nếu người dùng tồn tại và có số điện thoại
            if (user != null && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                context.Succeed(requirement);
            }
        }
    }
}