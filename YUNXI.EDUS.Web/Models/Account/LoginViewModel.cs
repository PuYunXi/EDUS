using Abp.Auditing;
using System.ComponentModel.DataAnnotations;

namespace YUNXI.EDUS.Web.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        [DisableAuditing]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string TenancyName { get; set; }

        [Required]
        public string UsernameOrEmailAddress { get; set; }
    }
}