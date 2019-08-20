using Abp.MultiTenancy;
using Abp.Web.Mvc.Authorization;
using System.Threading.Tasks;
using System.Web.Mvc;
using YUNXI.EDUS.Authorization;

namespace YUNXI.EDUS.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : EDUSControllerBase
    {
        public async Task<ActionResult> Index()
        {
            if (this.AbpSession.MultiTenancySide == MultiTenancySides.Tenant)
            {
                if (await this.IsGrantedAsync(PermissionNames.Pages_Tenant_Dashboard))
                {
                    return this.RedirectToAction("Index", "Dashboard");
                }
            }

            // Default page if no permission to the pages above
            return this.RedirectToAction("Index", "Welcome");
        }
    }
}