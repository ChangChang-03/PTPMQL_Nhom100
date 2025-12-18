using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MVC.Models.ViewModels
{
    public class UserWithRoleVM
    {
        public IdentityUser User { get; set; }
        public IList<string> Roles { get; set; }

        //thêm
        public string PhoneNumber => User?.PhoneNumber;
    }
}
