using System.Threading.Tasks;
using Abp.Application.Services;
using YUNXI.EDUS.Configuration.Dto;

namespace YUNXI.EDUS.Configuration
{
    public interface IConfigurationAppService: IApplicationService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}