using Abp.Configuration.Startup;

namespace YUNXI.EDUS.YunxiEdusCoreConfigurations
{
    public static class YunxiEdusCoreConfigurationExtensions
    {
        public static IYunxiEdusCoreConfiguration YunxiEdusCore(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IYunxiEdusCoreConfiguration>();
        }
    }

   
}
