using System.Collections.Generic;
using System.Security.Claims;

namespace MVC.Models.ViewModels
{
    public class UserClaimVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<Claim> UserClaims { get; set; }

        public UserClaimVM(string userId, string userName, List<Claim> userClaims)
        {
            UserId = userId;
            UserName = userName;
            UserClaims = userClaims;
        }
    }
}