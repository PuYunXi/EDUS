using Abp.Web.Mvc.Views;

namespace YUNXI.EDUS.Web.Views
{
    public abstract class EDUSWebViewPageBase : EDUSWebViewPageBase<dynamic>
    {

    }

    public abstract class EDUSWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected EDUSWebViewPageBase()
        {
            LocalizationSourceName = EDUSConsts.LocalizationSourceName;
        }
    }
}