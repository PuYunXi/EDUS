using Abp.Localization;
using System.Collections.Generic;

namespace YUNXI.EDUS.Web.Models.Account
{
    public class LanguagesViewModel
    {
        public IReadOnlyList<LanguageInfo> AllLanguages { get; set; }

        public LanguageInfo CurrentLanguage { get; set; }
    }
}