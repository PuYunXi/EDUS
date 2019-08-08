using System.Threading.Tasks;
using Abp.Application.Services;
using YUNXI.EDUS.Authorization.Accounts.Dto;

namespace YUNXI.EDUS.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
