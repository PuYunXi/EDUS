using Abp.MultiTenancy;
using YUNXI.EDUS.Authorization.Users;

namespace YUNXI.EDUS.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {
            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}