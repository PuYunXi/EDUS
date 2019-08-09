using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Net.Mail;
using Abp.Runtime.Security;
using System;
using System.Text;
using System.Threading.Tasks;
using YUNXI.EDUS.Emailing;
using YUNXI.EDUS.MultiTenancy;
using YUNXI.EDUS.Web;

namespace YUNXI.EDUS.Authorization.Users
{
    /// <summary>
    ///     Used to send email to users.
    /// </summary>
    public class UserEmailer : EDUSServiceBase, IUserEmailer, ITransientDependency
    {
        private readonly IEmailSender emailSender;

        private readonly IEmailTemplateProvider emailTemplateProvider;

        private readonly IRepository<Tenant> tenantRepository;

        private readonly ICurrentUnitOfWorkProvider unitOfWorkProvider;

        private readonly IWebUrlService webUrlService;

        public UserEmailer(
            IEmailTemplateProvider emailTemplateProvider,
            IEmailSender emailSender,
            IWebUrlService webUrlService,
            IRepository<Tenant> tenantRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider)
        {
            this.emailTemplateProvider = emailTemplateProvider;
            this.emailSender = emailSender;
            this.webUrlService = webUrlService;
            this.tenantRepository = tenantRepository;
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">
        /// User
        /// </param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [UnitOfWork]
        public virtual async Task SendEmailActivationLinkAsync(User user, string plainPassword = null)
        {
            if (user.EmailConfirmationCode.IsNullOrEmpty())
            {
                throw new ApplicationException(this.L("ShouldSetEmailConfirmationCode"));
            }

            var tenancyName = this.GetTenancyNameOrNull(user.TenantId);

            var link = this.webUrlService.GetSiteRootAddress(tenancyName) + "Account/EmailConfirmation" + "?userId="
                       + Uri.EscapeDataString(SimpleStringCipher.Instance.Encrypt(user.Id.ToString())) + "&tenantId="
                       + (user.TenantId == null
                              ? string.Empty
                              : Uri.EscapeDataString(
                                  SimpleStringCipher.Instance.Encrypt(user.TenantId.Value.ToString())))
                       + "&confirmationCode=" + Uri.EscapeDataString(user.EmailConfirmationCode);

            var emailTemplate = new StringBuilder(this.emailTemplateProvider.GetDefaultTemplate());
            emailTemplate.Replace("{EMAIL_TITLE}", this.L("EmailActivation_Title"));
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", this.L("EmailActivation_SubTitle"));

            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + this.L("NameSurname") + "</b>: " + user.Name + " " + user.Surname + "<br />");

            if (!tenancyName.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + this.L("TenancyName") + "</b>: " + tenancyName + "<br />");
            }

            mailMessage.AppendLine("<b>" + this.L("UserName") + "</b>: " + user.UserName + "<br />");

            if (!plainPassword.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + this.L("Password") + "</b>: " + plainPassword + "<br />");
            }

            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(this.L("EmailActivation_ClickTheLinkBelowToVerifyYourEmail") + "<br /><br />");
            mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");

            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

            await
                this.emailSender.SendAsync(
                    user.EmailAddress,
                    this.L("EmailActivation_Subject"),
                    emailTemplate.ToString());
        }

        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">
        /// User
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SendPasswordResetLinkAsync(User user)
        {
            if (user.PasswordResetCode.IsNullOrEmpty())
            {
                throw new ApplicationException(this.L("ShouldSetPasswordResetCode"));
            }

            var tenancyName = this.GetTenancyNameOrNull(user.TenantId);

            var link = this.webUrlService.GetSiteRootAddress(tenancyName) + "Account/ResetPassword" + "?userId="
                       + Uri.EscapeDataString(SimpleStringCipher.Instance.Encrypt(user.Id.ToString())) + "&resetCode="
                       + Uri.EscapeDataString(user.PasswordResetCode);

            var emailTemplate = new StringBuilder(this.emailTemplateProvider.GetDefaultTemplate());
            emailTemplate.Replace("{EMAIL_TITLE}", this.L("PasswordResetEmail_Title"));
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", this.L("PasswordResetEmail_SubTitle"));

            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + this.L("NameSurname") + "</b>: " + user.Name + " " + user.Surname + "<br />");

            if (!tenancyName.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + this.L("TenancyName") + "</b>: " + tenancyName + "<br />");
            }

            mailMessage.AppendLine("<b>" + this.L("UserName") + "</b>: " + user.UserName + "<br />");

            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(this.L("PasswordResetEmail_ClickTheLinkBelowToResetYourPassword") + "<br /><br />");
            mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");

            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

            await
                this.emailSender.SendAsync(
                    user.EmailAddress,
                    this.L("PasswordResetEmail_Subject"),
                    emailTemplate.ToString());
        }

        private string GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (this.unitOfWorkProvider.Current.SetTenantId(null))
            {
                return this.tenantRepository.Get(tenantId.Value).TenancyName;
            }
        }
    }
}
