using Abp.Web.Mvc.Authorization;
using System.Web.Mvc;

namespace YUNXI.EDUS.Web.Controllers
{
    [AbpMvcAuthorize]
    public class WelcomeController : EDUSControllerBase
    {
        public ActionResult Index()
        {
            return this.View();
        }
    }
}