using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using YUNXI.EDUS.Authorization.Users;

namespace YUNXI.EDUS.Web.Models.Account
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public bool IsExternalLogin { get; set; }

        [Required]
        [StringLength(User.MaxNameLength)]
        public string Name { get; set; }

        [StringLength(User.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        [Required]
        [StringLength(User.MaxSurnameLength)]
        public string Surname { get; set; }

        /// <summary>
        ///     Not required for single-tenant applications.
        /// </summary>
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }

        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var emailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            if (!this.UserName.Equals(this.EmailAddress) && emailRegex.IsMatch(this.UserName))
            {
                yield return
                    new ValidationResult(
                        "Username cannot be an email address unless it's same with your email address !");
            }
        }
    }
}