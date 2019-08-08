using System.Threading.Tasks;
using Abp.Application.Services;
using YUNXI.EDUS.Sessions.Dto;

namespace YUNXI.EDUS.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
