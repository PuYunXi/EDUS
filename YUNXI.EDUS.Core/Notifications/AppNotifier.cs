using Abp;
using Abp.Localization;
using Abp.Notifications;
using System.Threading.Tasks;
using YUNXI.EDUS.Authorization.Users;
using YUNXI.EDUS.MultiTenancy;

namespace YUNXI.EDUS.Notifications
{
    public class AppNotifier : EDUSDomainServiceBase, IAppNotifier
    {
        private readonly INotificationPublisher notificationPublisher;

        public AppNotifier(INotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;
        }

        public async Task NewTenantRegisteredAsync(Tenant tenant)
        {
            var notificationData =
                new LocalizableMessageNotificationData(
                    new LocalizableString("NewTenantRegisteredNotificationMessage", CoreConsts.LocalizationSourceName));

            notificationData["tenancyName"] = tenant.TenancyName;
            await this.notificationPublisher.PublishAsync(AppNotificationNames.NewTenantRegistered, notificationData);
        }

        public async Task NewUserRegisteredAsync(User user)
        {
            var notificationData =
                new LocalizableMessageNotificationData(
                    new LocalizableString("NewUserRegisteredNotificationMessage", CoreConsts.LocalizationSourceName));

            notificationData["userName"] = user.UserName;
            notificationData["emailAddress"] = user.EmailAddress;

            await
                this.notificationPublisher.PublishAsync(
                    AppNotificationNames.NewUserRegistered,
                    notificationData,
                    tenantIds: new[] { user.TenantId });
        }

        // This is for test purposes
        public async Task SendMessageAsync(
            UserIdentifier user,
            string message,
            NotificationSeverity severity = NotificationSeverity.Info)
        {
            await
                this.notificationPublisher.PublishAsync(
                    "App.SimpleMessage",
                    new MessageNotificationData(message),
                    severity: severity,
                    userIds: new[] { user });
        }

        public async Task WelcomeToTheApplicationAsync(User user)
        {
            await
                this.notificationPublisher.PublishAsync(
                    AppNotificationNames.WelcomeToTheApplication,
                    new MessageNotificationData(this.L("WelcomeToTheApplicationNotificationMessage")),
                    severity: NotificationSeverity.Success,
                    userIds: new[] { user.ToUserIdentifier() });
        }
    }
}
