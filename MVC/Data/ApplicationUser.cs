using Microsoft.AspNetCore.Identity;

namespace MVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string MaSinhVien { get; set; } // mã sinh tự động
    }
}
