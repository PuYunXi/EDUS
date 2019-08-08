using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using YUNXI.EDUS.EntityFramework;

namespace YUNXI.EDUS.Migrator
{
    [DependsOn(typeof(EDUSDataModule))]
    public class EDUSMigratorModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer<EDUSDbContext>(null);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}