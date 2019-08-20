using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using YUNXI.EDUS.Authorization.Users;
using YUNXI.EDUS.MultiTenancy;

namespace YUNXI.EDUS
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class EDUSAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected EDUSAppServiceBase()
        {
            LocalizationSourceName = EDUSConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = this.UserManager.FindByIdAsync(this.AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException(this.L("ThereIsNoCurrentUser"));
            }

            return user;
        }
        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}