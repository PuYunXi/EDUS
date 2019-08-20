using System.Web.Optimization;
namespace YUNXI.EDUS.Web.Bundling
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            AddMpaCssLibs(bundles);

            bundles.Add(
                new ScriptBundle("~/Bundles/Foundation/js").Include(
                    ScriptPaths.Json2,
                    ScriptPaths.JQuery2,
                    ScriptPaths.JQueryValidation,
                    ScriptPaths.Bootstrap,
                    ScriptPaths.JQueryBlockUi,
                    ScriptPaths.JQueryCookie,
                    ScriptPaths.JQueryAjaxForm,
                    ScriptPaths.SignalR,
                    ScriptPaths.SpinJs,
                    ScriptPaths.SpinJsJQuery,
                    ScriptPaths.SweetAlert,
                    ScriptPaths.Toastr,
                    ScriptPaths.MomentJs,
                    ScriptPaths.MomentTimezoneJs,
                    ScriptPaths.Underscore,
                    ScriptPaths.Handlebars).ForceOrdered());

            bundles.Add(
                new ScriptBundle("~/Bundles/AbpStaff/js").Include(
                    ScriptPaths.Abp,
                    ScriptPaths.AbpJQuery,
                    ScriptPaths.AbpUtils,
                    ScriptPaths.AbpBlockUi,
                    ScriptPaths.AbpModalManager,
                    ScriptPaths.AbpToastr,
                    ScriptPaths.AbpSpinJs,
                    ScriptPaths.AbpSweetAlert,
                    ScriptPaths.AbpMoment,
                    ScriptPaths.AppUserNotificationHelper,
                    ScriptPaths.AbpJqueryCustom));

            #region # ~/Bundles/Plugins/js

            bundles.Add(
                new ScriptBundle("~/Bundles/Plugins/js").Include(
                    ScriptPaths.JQueryDatatables,
                    ScriptPaths.BootstrapDatatables,
                    ScriptPaths.BootstrapDatatablesResponsive,
                    ScriptPaths.BootstrapHoverDropdown,
                    ScriptPaths.JQuerySlimscroll,
                    ScriptPaths.JsTree,
                    ScriptPaths.BootstrapSwitch,
                    ScriptPaths.DateRangePicker,
                    ScriptPaths.BootstrapSelect,
                    ScriptPaths.Select2,
                    ScriptPaths.Dotdotdot,
                    ScriptPaths.FullScreen,
                    ScriptPaths.Layer,
                    ScriptPaths.FastClick).ForceOrdered());

            #endregion

            #region # ~/Bundles/AppWimi/js

            bundles.Add(
                new ScriptBundle("~/Bundles/AppWimi/js").Include(
                    ScriptPaths.App,
                    ScriptPaths.AppConsts,
                    ScriptPaths.AppHelpers,
                    ScriptPaths.AppPluginSettings,
                    ScriptPaths.AppWimi).Include("~/Views/Common/Modals/_LookupModal.js").ForceOrdered());

            #endregion

            #region # ~/Bundles/charts/js & ~/Bundles/d3/js

            bundles.Add(new ScriptBundle("~/Bundles/charts/js").Include(ScriptPaths.Echarts));

            bundles.Add(new ScriptBundle("~/Bundles/d3/js").Include(ScriptPaths.D3).ForceOrdered());

            #endregion

            #region # ~/Bundles/pic/js

            bundles.Add(new ScriptBundle("~/Bundles/pic/js").Include(ScriptPaths.JQueryColor, ScriptPaths.JQueryJcrop));

            #endregion

            BundleTable.EnableOptimizations = false;
        }

        private static void AddMpaCssLibs(BundleCollection bundles)
        {
            bundles.Add(
                new StyleBundle("~/Content/css/bundle").Include(StylePaths.Bootstrap)
                    .Include(StylePaths.FontAwesome)
                    .Include(StylePaths.BootstrapDatatables, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.BootstrapDatatablesResponsive)
                    .Include(StylePaths.FamFamFamFlags)
                    .Include(StylePaths.JsTree)
                    .Include(StylePaths.SweetAlert)
                    .Include(StylePaths.Toastr)
                    .Include(StylePaths.Select2)
                    .Include(StylePaths.BootstrapSelect)
                    .Include(StylePaths.BootstrapSwitch)
                    .Include(StylePaths.BootstrapDateRangePicker)
                    .Include(StylePaths.AdminLte)
                    .Include(StylePaths.SkinBlue)
                    .Include(StylePaths.App)
                    .ForceOrdered());

            bundles.Add(
                new StyleBundle("~/Content/css/jcrop").Include(
                    StylePaths.JQueryJcrop,
                    new CssRewriteUrlWithVirtualDirectoryTransform()));
        }
    }
}