using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;

namespace YUNXI.EDUS.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : EDUSControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}