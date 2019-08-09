using Abp.Dependency;
using Abp.IO.Extensions;
using System.Reflection;
using System.Text;

namespace YUNXI.EDUS.Emailing
{
    class EmailTemplateProvider : IEmailTemplateProvider, ITransientDependency
    {
        public string GetDefaultTemplate()
        {
            using (
               var stream =
                   Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream("YUNXI.EDUS.Emailing.EmailTemplates.default.html"))
            {
                var bytes = stream.GetAllBytes();
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            }
        }
    }
}
