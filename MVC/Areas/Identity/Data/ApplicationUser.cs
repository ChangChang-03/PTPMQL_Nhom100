using Microsoft.AspNetCore.Identity;

namespace MVC.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } 
        
    }
}

