using Abp.Web.Mvc.Views;
using System.Diagnostics.CodeAnalysis;

namespace YUNXI.EDUS.Web.Views
{
    public abstract class EDUSWebViewPageBase : EDUSWebViewPageBase<dynamic>
    {

    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class EDUSWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected EDUSWebViewPageBase()
        {
            LocalizationSourceName = CoreConsts.LocalizationSourceName;
        }
    }
}