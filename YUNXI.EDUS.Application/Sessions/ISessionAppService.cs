using Abp.Application.Services;
using System.Threading.Tasks;
using YUNXI.EDUS.AppSystem.Sessions.Dto;

namespace YUNXI.EDUS.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutputDto> GetCurrentLoginInformations();
    }
}
