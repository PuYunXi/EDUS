using Abp.Authorization;
using Abp.Dependency;
using Abp.UI;
using System;

namespace YUNXI.EDUS.Authorization
{
    public class AbpLoginResultTypeHelper : EDUSServiceBase, ITransientDependency
    {
        public Exception CreateExceptionForFailedLoginAttempt(
            AbpLoginResultType result,
            string usernameOrEmailAddress,
            string tenancyName)
        {
            switch (result)
            {
                case AbpLoginResultType.Success:
                    return new ApplicationException(this.L("DonotCallThisMethodWithASuccessResult"));
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                case AbpLoginResultType.InvalidPassword:
                    return new UserFriendlyException(this.L("LoginFailed"), this.L("InvalidUserNameOrPassword"));
                case AbpLoginResultType.InvalidTenancyName:
                    return new UserFriendlyException(
                        this.L("LoginFailed"),
                        this.L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
                case AbpLoginResultType.TenantIsNotActive:
                    return new UserFriendlyException(this.L("LoginFailed"), this.L("TenantIsNotActive{0}", tenancyName));
                case AbpLoginResultType.UserIsNotActive:
                    return new UserFriendlyException(
                        this.L("LoginFailed"),
                        this.L("UserIsNotActiveAndCanNotLogin{0}", usernameOrEmailAddress));
                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return new UserFriendlyException(
                        this.L("LoginFailed"),
                        this.L("UserEmailIsNotConfirmedAndCanNotLogin"));
                default:

                    // Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    this.Logger.Warn("Unhandled login fail reason: " + result);
                    return new UserFriendlyException(this.L("LoginFailed"));
            }
        }
    }
}
