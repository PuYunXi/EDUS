using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Abp.Zero.EntityFramework;
using YUNXI.EDUS.EntityFramework;

namespace YUNXI.EDUS
{
    [DependsOn(typeof(AbpZeroEntityFrameworkModule), typeof(EDUSCoreModule))]
    public class EDUSDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<EDUSDbContext>());

            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
