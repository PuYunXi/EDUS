using Abp.Auditing;
using Abp.AutoMapper;
using System.Threading.Tasks;
using YUNXI.EDUS.AppSystem.Sessions.Dto;
using YUNXI.EDUS.Sessions.Dto;

namespace YUNXI.EDUS.Sessions
{
    public class SessionAppService : EDUSAppServiceBase, ISessionAppService
    {
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutputDto> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutputDto
            {
                User =
                                     (await GetCurrentUserAsync())
                                     .MapTo<UserLoginInfoDto>()
            };

            if (this.AbpSession.TenantId.HasValue)
            {
                output.Tenant = (await this.GetCurrentTenantAsync()).MapTo<TenantLoginInfoDto>();
            }

            return output;
        }

    }
}