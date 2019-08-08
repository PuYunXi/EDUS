using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using YUNXI.EDUS.Configuration.Dto;

namespace YUNXI.EDUS.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : EDUSAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
