using Abp.Extensions;
using System.IO;
using System.Threading;
using System.Web;

namespace YUNXI.EDUS.Web.Bundling
{
    public static class ScriptPaths
    {
        // public const string MustacheJs = "~/libs/mustachejs/mustache.min.js";
        public const string Abp = "~/Abp/Framework/scripts/abp.js";

        public const string AbpBlockUi = "~/Abp/Framework/scripts/libs/abp.blockUI.js";

        public const string AbpJQuery = "~/Abp/Framework/scripts/libs/abp.jquery.js";

        public const string AbpJqueryCustom = "~/Abp/Framework/scripts/jquery-custom.js";

        public const string AbpModalManager = "~/Abp/Framework/scripts/ModalManager.js";

        public const string AbpMoment = "~/Abp/Framework/scripts/libs/abp.moment.js";

        public const string AbpSpinJs = "~/Abp/Framework/scripts/libs/abp.spin.js";

        public const string AbpSweetAlert = "~/Abp/Framework/scripts/libs/abp.sweet-alert.js";

        public const string AbpToastr = "~/Abp/Framework/scripts/libs/abp.toastr.js";

        public const string AbpUtils = "~/Abp/Framework/scripts/utils.js";

        public const string App = "~/Scripts/app.js";

        public const string AppConsts = "~/Scripts/appConsts.js";

        public const string AppHelpers = "~/Scripts/appHelpers.js";

        public const string AppPluginSettings = "~/Scripts/appPluginSettings.js";

        public const string AppUserNotificationHelper = "~/Abp/Framework/scripts/appUserNotificationHelper.js";

        public const string AppWimi = "~/Scripts/appWimi.js";

        public const string Bootstrap = "~/Scripts/bootstrap.min.js";

        public const string BootstrapDatatables = "~/Scripts/datatables/dataTables.bootstrap.min.js";

        public const string BootstrapDatatablesResponsive =
            "~/Scripts/datatables/extensions/Responsive/js/dataTables.responsive.min.js";

        public const string BootstrapHoverDropdown = "~/Scripts/bootstrap-hover-dropdown.min.js";

        public const string BootstrapSelect = "~/Scripts/bootstrap-select/bootstrap-select.min.js";

        public const string BootstrapSwitch = "~/Scripts/bootstrap-switch.min.js";

        public const string D3 = "~/Scripts/d3.min.js";

        public const string D3SpecialGrannt = "~/Scripts/d3-s-grantt.js";

        public const string DateRangePicker = "~/Scripts/daterangepicker.js";

        public const string Dotdotdot = "~/Scripts/jquery.Dotdotdot.min.js";

        public const string Echarts = "~/Scripts/echarts.js";

        public const string FastClick = "~/Scripts/fastclick.js";

        public const string FullScreen = "~/Scripts/jquery.fullscreen.min.js";

        public const string Handlebars = "~/Scripts/handlebars-v4.0.5.js";

        public const string JQuery2 = "~/Scripts/jquery-2.2.0.min.js";

        public const string JQueryAjaxForm = "~/Scripts/jquery.form.js";

        public const string JQueryBlockUi = "~/Scripts/jquery.blockui.js";

        public const string JQueryColor = "~/Scripts/jquery.color.js";

        public const string JQueryCookie = "~/Scripts/jquery.cookie.min.js";

        public const string JQueryDatatables = "~/Scripts/datatables/jquery.dataTables.js";

        public const string JQueryJcrop = "~/Scripts/jquery.Jcrop.min.js";

        public const string JQuerySlimscroll = "~/Scripts/jquery.slimscroll.min.js";

        public const string JQueryUi = "~/Scripts/jquery-ui-1.11.4.min.js";

        public const string JQueryValidation = "~/Scripts/jquery.validate.min.js";

        public const string Json2 = "~/Scripts/json2.min.js";

        public const string JsTree = "~/Scripts/jstree/jstree.min.js";

        public const string MomentJs = "~/Scripts/moment-with-locales.min.js";

        public const string MomentTimezoneJs = "~/Scripts/moment-timezone-with-data.min.js";

        public const string Select2 = "~/Scripts/select2.min.js";

        public const string SignalR = "~/Scripts/jquery.signalR-2.2.0.min.js";

        public const string Layer = "~/Scripts/layer/layer.js";

        // loading animation
        public const string SpinJs = "~/Scripts/others/spinjs/spin.js";

        public const string SpinJsJQuery = "~/Scripts/others/spinjs/jquery.spin.js";

        public const string SweetAlert = "~/Scripts/sweet-alert.min.js";

        public const string Toastr = "~/Scripts/toastr.min.js";

        public const string Underscore = "~/Scripts/underscore.min.js";

        public static string JQueryValidationLocalization
            =>
                GetLocalizationFileForjQueryValidationOrNull(
                    Thread.CurrentThread.CurrentUICulture.Name.ToLower().Replace("-", "_"))
                ?? GetLocalizationFileForjQueryValidationOrNull(
                    Thread.CurrentThread.CurrentUICulture.Name.Left(2).ToLower())
                ?? "~/Scripts/jquery-validation/localization/_messages_empty.js";

        private static string GetLocalizationFileForjQueryValidationOrNull(string cultureCode)
        {
            try
            {
                var relativeFilePath = "~/Scripts/jquery-validation/localization/messages_" + cultureCode + ".min.js";
                var physicalFilePath = HttpContext.Current.Server.MapPath(relativeFilePath);
                if (File.Exists(physicalFilePath))
                {
                    return relativeFilePath;
                }
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}