using Abp.Application.Navigation;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.Web.Mvc.Authorization;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web.Mvc;
using YUNXI.EDUS.Sessions;
using YUNXI.EDUS.Web.Models.Languages;
using YUNXI.EDUS.Web.Models.Layout;

namespace YUNXI.EDUS.Web.Controllers
{
    [AbpMvcAuthorize]
    public class LayoutController : EDUSControllerBase
    {
        private readonly ILanguageManager languageManager;

        private readonly IMultiTenancyConfig multiTenancyConfig;

        private readonly ISessionAppService sessionAppService;

        private readonly IUserNavigationManager userNavigationManager;

        public LayoutController(
            ISessionAppService sessionAppService,
            IUserNavigationManager userNavigationManager,
            IMultiTenancyConfig multiTenancyConfig,
            ILanguageManager languageManager)
        {
            this.sessionAppService = sessionAppService;
            this.userNavigationManager = userNavigationManager;
            this.multiTenancyConfig = multiTenancyConfig;
            this.languageManager = languageManager;
        }

        [ChildActionOnly]
        public PartialViewResult Footer()
        {
            var footerModel = new FooterViewModel
            {
                LoginInformations =
                                          AsyncHelper.RunSync(
                                              () =>
                                              this.sessionAppService.GetCurrentLoginInformations())
            };

            return this.PartialView("_Footer", footerModel);
        }

        public ActionResult GetSentinelLdkFeatures()
        {
            var url =
                @"http://localhost:1947/_int_/tab_feat.html?haspid=0&featureid=-1&vendorid=0&productid=0&filterfrom=1&filterto=10";

            HttpWebResponse response;

            try
            {
                response = HttpWebResponseUtility.CreateGetHttpResponse(url, 600, null, null);

                var respStream = response.GetResponseStream();

                Debug.Assert(respStream != null, "respStream != null");
                using (var reader = new System.IO.StreamReader(respStream, Encoding.UTF8))
                {
                    var responseText = reader.ReadToEnd();
                    return this.Content(responseText);
                }
            }
            catch
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [ChildActionOnly]
        public PartialViewResult LanguageSelection()
        {
            var model = new LanguageSelectionViewModel
            {
                CurrentLanguage = languageManager.CurrentLanguage,
                Languages = languageManager.GetLanguages()
            };

            return PartialView("_LanguageSelection", model);
        }

        [ChildActionOnly]
        public PartialViewResult Header()
        {
            var headerModel = new HeaderViewModel
            {
                LoginInformations =
                                          AsyncHelper.RunSync(
                                              () =>
                                              this.sessionAppService.GetCurrentLoginInformations()),
                Languages = this.languageManager.GetLanguages(),
                CurrentLanguage = this.languageManager.CurrentLanguage,
                IsMultiTenancyEnabled = this.multiTenancyConfig.IsEnabled,
                IsImpersonatedLogin = this.AbpSession.ImpersonatorUserId.HasValue
            };
            return this.PartialView("_Header", headerModel);
        }

        [ChildActionOnly]
        public PartialViewResult Sidebar(string currentPageName = "")
        {
            var sidebarModel = new SidebarViewModel
            {
                Menu =
                                           AsyncHelper.RunSync(
                                               () =>
                                               this.userNavigationManager.GetMenuAsync(
                                                   EDUSNavigationProvider.MenuName,
                                                   this.AbpSession.ToUserIdentifier())),
                CurrentPageName = currentPageName
            };

            return this.PartialView("_Sidebar", sidebarModel);
        }

        public void ToggleSidebarCollapse()
        {
            this.Session["sidebar-collapse"] = this.Session["sidebar-collapse"] == null ? "sidebar-collapse" : null;
        }
    }
}