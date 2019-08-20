using Abp.Extensions;
using System.Web;
using System.Web.Optimization;

namespace YUNXI.EDUS.Web.Bundling
{
    public class CssRewriteUrlWithVirtualDirectoryTransform : IItemTransform
    {
        private readonly CssRewriteUrlTransform rewriteUrlTransform;

        public CssRewriteUrlWithVirtualDirectoryTransform()
        {
            this.rewriteUrlTransform = new CssRewriteUrlTransform();
        }

        public string Process(string includedVirtualPath, string input)
        {
            var result = this.rewriteUrlTransform.Process(includedVirtualPath, input);

            if (!HttpRuntime.AppDomainAppVirtualPath.IsNullOrEmpty() && HttpRuntime.AppDomainAppVirtualPath != "/")
            {
                result = result.Replace(@"url(/", @"url(" + HttpRuntime.AppDomainAppVirtualPath + @"/");
            }

            return result;
        }
    }
}