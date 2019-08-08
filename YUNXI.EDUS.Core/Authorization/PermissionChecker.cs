using Abp.Authorization;
using YUNXI.EDUS.Authorization.Roles;
using YUNXI.EDUS.Authorization.Users;

namespace YUNXI.EDUS.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
