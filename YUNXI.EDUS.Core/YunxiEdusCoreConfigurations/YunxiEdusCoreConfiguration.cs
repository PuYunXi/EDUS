using Abp.Collections;
using YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes;

namespace YUNXI.EDUS.YunxiEdusCoreConfigurations
{
    public class YunxiEdusCoreConfiguration : IYunxiEdusCoreConfiguration
    {
        public YunxiEdusCoreConfiguration()
        {
            this.NotificationTypeProviders = new TypeList<NotificationTypeProvider>();
        }
        public ITypeList<NotificationTypeProvider> NotificationTypeProviders { get; private set; }
    }
}
