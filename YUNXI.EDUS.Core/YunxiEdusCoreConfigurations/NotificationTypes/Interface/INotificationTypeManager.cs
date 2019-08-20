using System.Collections.Generic;

namespace YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes.Interface
{
    public interface INotificationTypeManager
    {
        IDictionary<string, NotificationTypeDefinition> Types { get; }
    }
}
