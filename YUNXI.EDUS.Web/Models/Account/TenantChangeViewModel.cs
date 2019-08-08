using Abp.AutoMapper;
using YUNXI.EDUS.Sessions.Dto;

namespace YUNXI.EDUS.Web.Models.Account
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}