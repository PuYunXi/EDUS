using System.Web.Optimization;

namespace YUNXI.EDUS.Web.Bundling
{
    public static class CommonBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // COMMON BUNDLES USED BOTH IN FRONTEND AND BACKEND
            bundles.Add(
                new StyleBundle("~/Bundles/Common/css").IncludeDirectory("~/Common/Styles", "*.css", true)
                    .ForceOrdered());

            bundles.Add(
                new ScriptBundle("~/Bundles/Abp/js").IncludeDirectory("~/Common/Scripts", "*.js", true).ForceOrdered());
        }
    }
}