using Abp.Localization;
using System.Collections.Generic;
using YUNXI.EDUS.AppSystem.Sessions.Dto;

namespace YUNXI.EDUS.Web.Models.Layout
{
    public class HeaderViewModel
    {
        public LanguageInfo CurrentLanguage { get; set; }

        public bool IsImpersonatedLogin { get; set; }

        public bool IsMultiTenancyEnabled { get; set; }

        public IReadOnlyList<LanguageInfo> Languages { get; set; }

        public GetCurrentLoginInformationsOutputDto LoginInformations { get; set; }

        public string GetShownLoginName()
        {
            var userName = "<span id=\"HeaderCurrentUserName\">" + this.LoginInformations.User.UserName + "</span>";

            if (!this.IsMultiTenancyEnabled)
            {
                return userName;
            }

            return this.LoginInformations.Tenant == null
                       ? ".\\" + userName
                       : this.LoginInformations.Tenant.TenancyName + "\\" + userName;
        }
    }
}