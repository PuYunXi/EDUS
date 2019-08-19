using Abp.Configuration;
using System.Collections.Generic;
using System.Configuration;

namespace YUNXI.EDUS.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),
                new SettingDefinition(
                               AppSettings.General.WebSiteRootAddress,
                               ConfigurationManager.AppSettings[AppSettings.General.WebSiteRootAddress] ?? "http://localhost:6240/"),
            };
        }
    }
}