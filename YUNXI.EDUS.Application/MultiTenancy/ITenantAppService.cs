using Abp.Application.Services;
using Abp.Application.Services.Dto;
using YUNXI.EDUS.MultiTenancy.Dto;

namespace YUNXI.EDUS.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
