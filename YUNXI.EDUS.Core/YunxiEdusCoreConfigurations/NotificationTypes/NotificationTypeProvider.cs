using Abp.Dependency;
using YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes.Interface;

namespace YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes
{
    public abstract class NotificationTypeProvider : ISingletonDependency
    {
        public abstract void SetNotification(INotificationTypeProviderContext context);
    }
}
