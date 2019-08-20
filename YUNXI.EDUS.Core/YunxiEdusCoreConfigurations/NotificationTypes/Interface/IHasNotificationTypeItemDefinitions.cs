using System.Collections.Generic;

namespace YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes.Interface
{
    public interface IHasNotificationTypeItemDefinitions
    {
        IList<NotificationTypeDefinition> Items { get; set; }
    }
}
