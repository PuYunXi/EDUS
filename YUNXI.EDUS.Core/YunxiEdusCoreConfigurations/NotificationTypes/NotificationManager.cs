using Abp.Dependency;
using Abp.Localization;
using System.Collections.Generic;
using YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes.Interface;

namespace YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes
{
    public class NotificationManager : INotificationTypeManager, ISingletonDependency
    {
        private const string Mpa = "Main menu";

        private readonly IYunxiEdusCoreConfiguration configuration;

        private readonly IIocResolver iocResolver;

        public NotificationManager(IIocResolver iocResolver, IYunxiEdusCoreConfiguration configuration)
        {
            this.iocResolver = iocResolver;

            this.configuration = configuration;

            this.Types = new Dictionary<string, NotificationTypeDefinition>
                             {
                                 {
                                     CoreConsts.NotificationTypesName,
                                     new NotificationTypeDefinition(
                                     CoreConsts.NotificationTypesName,
                                     new FixedLocalizableString(Mpa))
                                 }
                             };
        }

        public NotificationTypeDefinition DefaultTypes
        {
            get
            {
                return this.Types[CoreConsts.NotificationTypesName];
            }
        }

        public IDictionary<string, NotificationTypeDefinition> Types { get; }

        public void Initialize()
        {
            var context = new NotificationTypeProviderContext(this);
            foreach (var providerType in this.configuration.NotificationTypeProviders)
            {
                var provider = (NotificationTypeProvider)this.iocResolver.Resolve(providerType);
                provider.SetNotification(context);
            }
        }
    }
}
