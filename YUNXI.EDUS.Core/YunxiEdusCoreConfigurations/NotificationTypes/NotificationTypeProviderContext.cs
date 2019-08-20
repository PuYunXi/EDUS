using YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes.Interface;

namespace YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes
{
    public class NotificationTypeProviderContext : INotificationTypeProviderContext
    {
        public NotificationTypeProviderContext(INotificationTypeManager manager)
        {
            this.Manager = manager;
        }

        public INotificationTypeManager Manager { get; }
    }
}
