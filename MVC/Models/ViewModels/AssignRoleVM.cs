using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MVC.Models.ViewModels
{
    public class AssignRoleVM
    {
        public string UserId { get; set; }
        public IList<string> SelectedRoles { get; set; }
        public IList<RoleVM>? AllRoles { get; set; }
    }

    public class RoleVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}