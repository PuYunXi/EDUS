using Abp.Application.Navigation;

namespace YUNXI.EDUS.Web.Models.Layout
{
    public class SidebarViewModel
    {
        public string CurrentPageName { get; set; }

        public UserMenu Menu { get; set; }
    }
}