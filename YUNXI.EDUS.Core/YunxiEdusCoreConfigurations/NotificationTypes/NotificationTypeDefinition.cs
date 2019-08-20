using Abp.Localization;
using System;
using System.Collections.Generic;
using YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes.Interface;

namespace YUNXI.EDUS.YunxiEdusCoreConfigurations.NotificationTypes
{
    public sealed class NotificationTypeDefinition : IHasNotificationTypeItemDefinitions
    {
        public NotificationTypeDefinition(string name, ILocalizableString displayName, NotificationTriggerMode triggerMode = NotificationTriggerMode.ByTime)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), LocalizationHelper.GetString(CoreConsts.LocalizationSourceName, "MenuNameCannotBeNull"));
            }

            if (displayName == null)
            {
                throw new ArgumentNullException(nameof(displayName), LocalizationHelper.GetString(CoreConsts.LocalizationSourceName, "MenuDisplayNameCannotBeNull"));
            }

            this.Name = name;
            this.DisplayName = displayName;
            this.TriggerMode = triggerMode;

            this.Items = new List<NotificationTypeDefinition>();
        }

        public ILocalizableString DisplayName { get; set; }

        public IList<NotificationTypeDefinition> Items { get; set; }

        public string Name { get; set; }

        public NotificationTriggerMode TriggerMode { get; set; }

        public NotificationTypeDefinition AddItem(NotificationTypeDefinition item)
        {
            this.Items.Add(item);
            return this;
        }
    }
}
