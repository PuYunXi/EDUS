using Abp.IO;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Localization.Sources;
using Abp.Modules;
using Abp.Zero;
using Abp.Zero.Configuration;
using System.Reflection;
using System.Web;
using YUNXI.EDUS.Authorization;
using YUNXI.EDUS.Authorization.Roles;
using YUNXI.EDUS.Authorization.Users;
using YUNXI.EDUS.Configuration;
using YUNXI.EDUS.MultiTenancy;

namespace YUNXI.EDUS
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class EDUSCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            //Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            //Remove the following line to disable multi-tenancy.
            Configuration.MultiTenancy.IsEnabled = EDUSConsts.MultiTenancyEnabled;

            if (HttpContext.Current != null)
            {
                var langugePath = HttpContext.Current.Server.MapPath("~/Language");
                DirectoryHelper.CreateIfNotExists(langugePath);

                this.Configuration.Localization.Sources.Extensions.Add(new LocalizationSourceExtensionInfo("Language",
                    new XmlFileLocalizationDictionaryProvider(langugePath)));
            }



            //Add/remove localization sources here
            this.Configuration.Localization.Sources.Add(
      new DictionaryBasedLocalizationSource(
          "Language",
          new XmlEmbeddedFileLocalizationDictionaryProvider(
              Assembly.GetExecutingAssembly(),
                        "YUNXI.EDUS.Localization.Source")));



            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Authorization.Providers.Add<EDUSAuthorizationProvider>();

            Configuration.Settings.Providers.Add<AppSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
