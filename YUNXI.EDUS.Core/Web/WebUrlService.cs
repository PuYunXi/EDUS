using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using YUNXI.EDUS.Configuration;

namespace YUNXI.EDUS.Web
{
    public class WebUrlService : IWebUrlService, ITransientDependency
    {
        public const string TenancyNamePlaceHolder = "{TENANCY_NAME}";

        private readonly ISettingManager settingManager;

        public WebUrlService(ISettingManager settingManager)
        {
            this.settingManager = settingManager;
        }

        public string GetSiteRootAddress(string tenancyName = null)
        {
            var siteRootFormat =
                this.settingManager.GetSettingValue(AppSettings.General.WebSiteRootAddress).EnsureEndsWith('/');

            if (!siteRootFormat.Contains(TenancyNamePlaceHolder))
            {
                return siteRootFormat;
            }

            if (siteRootFormat.Contains(TenancyNamePlaceHolder + "."))
            {
                siteRootFormat = siteRootFormat.Replace(TenancyNamePlaceHolder + ".", TenancyNamePlaceHolder);
            }

            if (tenancyName.IsNullOrEmpty())
            {
                return siteRootFormat.Replace(TenancyNamePlaceHolder, string.Empty);
            }

            return siteRootFormat.Replace(TenancyNamePlaceHolder, tenancyName + ".");
        }
    }
}
