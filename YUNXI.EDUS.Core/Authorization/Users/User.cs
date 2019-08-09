using Abp.Authorization.Users;
using Abp.Extensions;
using Microsoft.AspNet.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace YUNXI.EDUS.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const int MaxWeChatIdLength = 50;

        public const int MinPlainPasswordLength = 6;

        public virtual Guid? ProfilePictureId { get; set; }

        public virtual bool ShouldChangePasswordOnNextLogin { get; set; }

        [StringLength(MaxWeChatIdLength)]
        public virtual string WeChatId { get; set; }

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        /// <summary>
        ///     Creates admin <see cref="User" /> for a tenant.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="emailAddress">Email address</param>
        /// <param name="password">Password</param>
        /// <returns>Created <see cref="User" /> object</returns>
        public static User CreateTenantAdminUser(int tenantId, string emailAddress, string password)
        {
            return new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Password = new PasswordHasher().HashPassword(password)
            };
        }

        public void Unlock()
        {
            this.AccessFailedCount = 0;
            this.LockoutEndDateUtc = null;
        }
    }
}