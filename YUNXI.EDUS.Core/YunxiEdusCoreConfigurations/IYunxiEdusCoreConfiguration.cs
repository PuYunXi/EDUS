using Abp.Collections;
using YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes;

namespace YUNXI.EDUS.YunxiEdusCoreConfigurations
{
    public interface IYunxiEdusCoreConfiguration
    {
        ITypeList<NotificationTypeProvider> NotificationTypeProviders { get; }
    }
}
