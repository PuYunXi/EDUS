using Abp;
using Abp.Notifications;
using System.Threading.Tasks;
using YUNXI.EDUS.Authorization.Users;
using YUNXI.EDUS.MultiTenancy;

namespace YUNXI.EDUS.Notifications
{
    public interface IAppNotifier
    {
        Task NewTenantRegisteredAsync(Tenant tenant);

        Task NewUserRegisteredAsync(User user);

        Task SendMessageAsync(
            UserIdentifier user,
            string message,
            NotificationSeverity severity = NotificationSeverity.Info);

        Task WelcomeToTheApplicationAsync(User user);
    }
}
