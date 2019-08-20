using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using YUNXI.EDUS.MultiTenancy;

namespace YUNXI.EDUS.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string EditionDisplayName { get; set; }

        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}