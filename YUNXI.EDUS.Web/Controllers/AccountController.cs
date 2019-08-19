namespace YUNXI.EDUS.Web.Controllers
{
    using Abp.Authorization;
    using Abp.Authorization.Users;
    using Abp.AutoMapper;
    using Abp.Configuration;
    using Abp.Configuration.Startup;
    using Abp.Domain.Uow;
    using Abp.Extensions;
    using Abp.Localization;
    using Abp.MultiTenancy;
    using Abp.Notifications;
    using Abp.Runtime.Caching;
    using Abp.Runtime.Security;
    using Abp.Runtime.Session;
    using Abp.Threading;
    using Abp.Timing;
    using Abp.UI;
    using Abp.Web.Models;
    using Abp.Web.Mvc.Authorization;
    using Abp.Zero.Configuration;
    using CaptchaMvc.HtmlHelpers;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using YUNXI.EDUS.Authorization;
    using YUNXI.EDUS.Authorization.Impersonation;
    using YUNXI.EDUS.Authorization.Roles;
    using YUNXI.EDUS.Authorization.Users;
    using YUNXI.EDUS.Configuration;
    using YUNXI.EDUS.Debugging;
    using YUNXI.EDUS.MultiTenancy;
    using YUNXI.EDUS.Notifications;
    using YUNXI.EDUS.Web;
    using YUNXI.EDUS.Web.Auth;
    using YUNXI.EDUS.Web.Controllers.Results;
    using YUNXI.EDUS.Web.Models.Account;
    using YUNXI.EDUS.Web.MultiTenancy;


    public class AccountController : EDUSControllerBase
    {
        private readonly AbpLoginResultTypeHelper abpLoginResultTypeHelper;

        private readonly IAppNotifier appNotifier;

        private readonly IAuthenticationManager authenticationManager;

        private readonly ICacheManager cacheManager;

        private readonly ILanguageManager languageManager;

        private readonly LogInManager logInManager;

        private readonly IMultiTenancyConfig multiTenancyConfig;

        private readonly INotificationSubscriptionManager notificationSubscriptionManager;

        private readonly RoleManager roleManager;

        private readonly ITenancyNameFinder tenancyNameFinder;

        private readonly TenantManager tenantManager;

        private readonly IUnitOfWorkManager unitOfWorkManager;

        private readonly IUserEmailer userEmailer;

        private readonly IUserLinkManager userLinkManager;

        private readonly UserManager userManager;

        private readonly IWebUrlService webUrlService;

        public AccountController(
            LogInManager logInManager,
            UserManager userManager,
            IMultiTenancyConfig multiTenancyConfig,
            IUserEmailer userEmailer,
            RoleManager roleManager,
            TenantManager tenantManager,
            IUnitOfWorkManager unitOfWorkManager,
            ITenancyNameFinder tenancyNameFinder,
            ICacheManager cacheManager,
            IAppNotifier appNotifier,
            IWebUrlService webUrlService,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            IUserLinkManager userLinkManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAuthenticationManager authenticationManager,
            ILanguageManager languageManager)
        {
            this.userManager = userManager;
            this.multiTenancyConfig = multiTenancyConfig;
            this.userEmailer = userEmailer;
            this.roleManager = roleManager;
            this.tenantManager = tenantManager;
            this.unitOfWorkManager = unitOfWorkManager;
            this.tenancyNameFinder = tenancyNameFinder;
            this.cacheManager = cacheManager;
            this.webUrlService = webUrlService;
            this.appNotifier = appNotifier;
            this.abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            this.userLinkManager = userLinkManager;
            this.notificationSubscriptionManager = notificationSubscriptionManager;
            this.authenticationManager = authenticationManager;
            this.languageManager = languageManager;
            this.logInManager = logInManager;
        }

        private IAuthenticationManager AuthenticationManager => this.HttpContext.GetOwinContext().Authentication;

        public virtual async Task<JsonResult> BackToImpersonator()
        {
            if (!this.AbpSession.ImpersonatorUserId.HasValue)
            {
                throw new UserFriendlyException(this.L("NotImpersonatedLoginErrorMessage"));
            }

            var result =
                await
                this.SaveImpersonationTokenAndGetTargetUrl(
                    this.AbpSession.ImpersonatorTenantId,
                    this.AbpSession.ImpersonatorUserId.Value,
                    true);
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return result;
        }

        public ActionResult EmailActivation()
        {
            this.ViewBag.IsMultiTenancyEnabled = this.multiTenancyConfig.IsEnabled;
            this.ViewBag.TenancyName = this.tenancyNameFinder.GetCurrentTenancyNameOrNull();

            return this.View();
        }

        [UnitOfWork]
        public virtual async Task<ActionResult> EmailConfirmation(EmailConfirmationViewModel model)
        {
            this.CheckModelState();

            var tenantId = model.TenantId.IsNullOrEmpty()
                               ? (int?)null
                               : SimpleStringCipher.Instance.Decrypt(model.TenantId).To<int>();
            var userId = Convert.ToInt64(SimpleStringCipher.Instance.Decrypt(model.UserId));

            this.unitOfWorkManager.Current.SetTenantId(tenantId);

            var user = await this.userManager.GetUserByIdAsync(userId);
            if (user == null || user.EmailConfirmationCode.IsNullOrEmpty()
                || user.EmailConfirmationCode != model.ConfirmationCode)
            {
                throw new UserFriendlyException(
                    this.L("InvalidEmailConfirmationCode"),
                    this.L("InvalidEmailConfirmationCode_Detail"));
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;

            await this.userManager.UpdateAsync(user);

            var tenancyName = user.TenantId.HasValue
                                  ? (await this.tenantManager.GetByIdAsync(user.TenantId.Value)).TenancyName
                                  : string.Empty;

            return this.RedirectToAction(
                "Login",
                new
                {
                    successMessage = this.L("YourEmailIsConfirmedMessage"),
                    tenancyName,
                    userNameOrEmailAddress = user.UserName
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(
                provider,
                this.Url.Action(
                    "ExternalLoginCallback",
                    "Account",
                    new
                    {
                        ReturnUrl = returnUrl,
                        tenancyName = this.tenancyNameFinder.GetCurrentTenancyNameOrNull() ?? string.Empty
                    }));
        }

        [UnitOfWork]
        public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl, string tenancyName = "")
        {
            var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return this.RedirectToAction("Login");
            }

            // Try to find tenancy name
            if (tenancyName.IsNullOrEmpty())
            {
                tenancyName = this.tenancyNameFinder.GetCurrentTenancyNameOrNull();
                if (tenancyName.IsNullOrEmpty())
                {
                    var tenants = await this.FindPossibleTenantsOfUserAsync(loginInfo.Login);
                    switch (tenants.Count)
                    {
                        case 0:
                            return await this.RegisterView(loginInfo);
                        case 1:
                            tenancyName = tenants[0].TenancyName;
                            break;
                        default:
                            return this.View(
                                "TenantSelection",
                                new TenantSelectionViewModel
                                {
                                    Action =
                                            this.Url.Action(
                                                "ExternalLoginCallback",
                                                "Account",
                                                new { returnUrl }),
                                    Tenants =
                                            tenants.MapTo<List<TenantSelectionViewModel.TenantInfo>>()
                                });
                    }
                }
            }

            var loginResult = await this.logInManager.LoginAsync(loginInfo.Login, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    this.SignInAsync(loginResult, true);

                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        returnUrl = this.Url.Action("Index", "Dashboard");
                    }

                    return this.Redirect(returnUrl);
                case AbpLoginResultType.UnknownExternalLogin:
                    return await this.RegisterView(loginInfo, tenancyName);
                default:
                    throw this.abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                        loginResult.Result,
                        loginInfo.Email ?? loginInfo.DefaultUserName,
                        tenancyName);
            }
        }

        public ActionResult ForgotPassword()
        {
            this.ViewBag.IsMultiTenancyEnabled = this.multiTenancyConfig.IsEnabled;
            this.ViewBag.TenancyName = this.tenancyNameFinder.GetCurrentTenancyNameOrNull();

            return this.View();
        }


        public virtual async Task<JsonResult> Impersonate(ImpersonateModel model)
        {
            this.CheckModelState();

            if (this.AbpSession.ImpersonatorUserId.HasValue)
            {
                throw new UserFriendlyException(this.L("CascadeImpersonationErrorMessage"));
            }

            if (this.AbpSession.TenantId.HasValue)
            {
                if (!model.TenantId.HasValue)
                {
                    throw new UserFriendlyException(this.L("FromTenantToHostImpersonationErrorMessage"));
                }

                if (model.TenantId.Value != this.AbpSession.TenantId.Value)
                {
                    throw new UserFriendlyException(this.L("DifferentTenantImpersonationErrorMessage"));
                }
            }

            var result = await this.SaveImpersonationTokenAndGetTargetUrl(model.TenantId, model.UserId, false);
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return result;
        }

        [UnitOfWork]
        public virtual async Task<ActionResult> ImpersonateSignIn(string tokenId)
        {
            var cacheItem = await this.cacheManager.GetImpersonationCache().GetOrDefaultAsync(tokenId);
            if (cacheItem == null)
            {
                throw new UserFriendlyException(this.L("ImpersonationTokenErrorMessage"));
            }

            // Switch to requested tenant
            this.unitOfWorkManager.Current.SetTenantId(cacheItem.TargetTenantId);

            // Get the user from tenant
            var user = await this.userManager.FindByIdAsync(cacheItem.TargetUserId);

            // Create identity
            var identity =
                await this.userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            if (!cacheItem.IsBackToImpersonator)
            {
                // Add claims for audit logging
                if (cacheItem.ImpersonatorTenantId.HasValue)
                {
                    identity.AddClaim(
                        new Claim(
                            AbpClaimTypes.ImpersonatorTenantId,
                            cacheItem.ImpersonatorTenantId.Value.ToString(CultureInfo.InvariantCulture)));
                }

                identity.AddClaim(
                    new Claim(
                        AbpClaimTypes.ImpersonatorUserId,
                        cacheItem.ImpersonatorUserId.ToString(CultureInfo.InvariantCulture)));
            }

            // Sign in with the target user
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            this.AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

            // Remove the cache item to prevent re-use
            await this.cacheManager.GetImpersonationCache().RemoveAsync(tokenId);

            return this.RedirectToAction("Index", "Dashboard");
        }

        public virtual JsonResult IsImpersonatedLogin()
        {
            return this.Json(new AjaxResponse { Result = this.AbpSession.ImpersonatorUserId.HasValue });
        }

        [ChildActionOnly]
        public PartialViewResult Languages()
        {
            return this.PartialView(
                "~/Views/Account/_Languages.cshtml",
                new LanguagesViewModel
                {
                    AllLanguages = this.languageManager.GetLanguages(),
                    CurrentLanguage = this.languageManager.CurrentLanguage
                });
        }

        public ActionResult Login(string userNameOrEmailAddress = "", string returnUrl = "", string successMessage = "")
        {
            this.ViewBag.ReturnUrl = returnUrl;
            this.ViewBag.IsMultiTenancyEnabled = this.multiTenancyConfig.IsEnabled;
            var vm = this.languageManager.GetLanguages();
            return
                this.View(
                    new LoginFormViewModel
                    {
                        TenancyName = this.tenancyNameFinder.GetCurrentTenancyNameOrNull(),
                        IsSelfRegistrationEnabled = this.IsSelfRegistrationEnabled(),
                        SuccessMessage = successMessage,
                        UserNameOrEmailAddress = userNameOrEmailAddress,
                        LanguageViewModel = new LanguagesViewModel
                        {
                            AllLanguages = this.languageManager.GetLanguages(),
                            CurrentLanguage = this.languageManager.CurrentLanguage
                        }

                    });
        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<JsonResult> Login(
            LoginViewModel loginModel,
            string returnUrl = "",
            string returnUrlHash = "")
        {
            this.CheckModelState();

            var loginResult =
                await
                this.GetLoginResultAsync(loginModel.UsernameOrEmailAddress, loginModel.Password, loginModel.TenancyName);

            var tenantId = loginResult.Tenant?.Id;

            using (this.UnitOfWorkManager.Current.SetTenantId(tenantId))

                if (loginResult.User.ShouldChangePasswordOnNextLogin)
                {
                    loginResult.User.SetNewPasswordResetCode();

                    return
                        this.Json(
                            new AjaxResponse
                            {
                                TargetUrl =
                                        this.Url.Action(
                                            "ResetPassword",
                                            new ResetPasswordViewModel
                                            {
                                                TenantId =
                                                        SimpleStringCipher.Instance
                                                        .Encrypt(
                                                            tenantId == null
                                                                ? null
                                                                : tenantId.ToString()),
                                                UserId =
                                                        SimpleStringCipher.Instance
                                                        .Encrypt(
                                                            loginResult.User.Id
                                                        .ToString()),
                                                ResetCode =
                                                        loginResult.User
                                                        .PasswordResetCode
                                            })
                            });
                }

            this.SignInAsync(loginResult, loginModel.RememberMe);
            await this.UnitOfWorkManager.Current.SaveChangesAsync();

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = this.Url.Action("Index", "Dashboard");
            }

            if (!string.IsNullOrWhiteSpace(returnUrlHash))
            {
                returnUrl = returnUrl + returnUrlHash;
            }

            return this.Json(new AjaxResponse { TargetUrl = returnUrl });
        }

        public virtual async Task<JsonResult> Login4Vision(LoginViewModel loginModel)
        {
            this.CheckModelState();
            var resultModel = new VisionEditorLoginResult() { Status = 0 };

            var loginResult =
                await
                this.logInManager.LoginAsync(
                    loginModel.UsernameOrEmailAddress,
                    loginModel.Password,
                    loginModel.TenancyName);

            if (loginResult.Result != AbpLoginResultType.Success)
            {
                switch (loginResult.Result)
                {
                    case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                    case AbpLoginResultType.InvalidPassword:
                        resultModel.Message = this.L("InvalidUserNameOrPassword");
                        resultModel.Status = -1;
                        break;
                    case AbpLoginResultType.UserIsNotActive:
                        resultModel.Message = this.L("UserIsNotActiveAndCanNotLogin", loginModel.UsernameOrEmailAddress);
                        resultModel.Status = -1;
                        break;

                    case AbpLoginResultType.UserEmailIsNotConfirmed:
                        resultModel.Message = this.L("UserEmailIsNotConfirmedAndCanNotLogin");
                        resultModel.Status = -1;
                        break;
                    default:

                        // Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                        this.Logger.Warn("Unhandled login fail reason: " + loginResult.Result);
                        resultModel.Message = this.L("LoginFailed");
                        resultModel.Status = -1;
                        break;
                }
            }

            if (loginResult.User.ShouldChangePasswordOnNextLogin)
            {
                resultModel.Message = this.L("NeedToChangePassword");
                resultModel.Status = -1;
            }

            this.SignInAsync(loginResult, loginModel.RememberMe);

            return this.Json(resultModel);
        }

        public ActionResult Logout()
        {
            this.AuthenticationManager.SignOut();
            return this.RedirectToAction("Login");
        }

        public ActionResult Register()
        {
            return
                this.RegisterView(
                    new RegisterViewModel { TenancyName = this.tenancyNameFinder.GetCurrentTenancyNameOrNull() });
        }

        [HttpPost]
        [UnitOfWork]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                this.CheckSelfRegistrationIsEnabled();

                this.CheckModelState();

                if (!model.IsExternalLogin && this.UseCaptchaOnRegistration())
                {
                    if (!this.IsCaptchaValid("Captcha is not valid"))
                    {
                        throw new UserFriendlyException(this.L("IncorrectCaptchaAnswer"));
                    }
                }

                if (!this.multiTenancyConfig.IsEnabled)
                {
                    model.TenancyName = Tenant.DefaultTenantName;
                }
                else if (model.TenancyName.IsNullOrEmpty())
                {
                    throw new UserFriendlyException(this.L("TenantNameCanNotBeEmpty"));
                }

                this.CurrentUnitOfWork.SetTenantId(null);

                var tenant = await this.GetActiveTenantAsync(model.TenancyName);

                this.CurrentUnitOfWork.SetTenantId(tenant.Id);

                if (
                    !await
                     this.SettingManager.GetSettingValueForTenantAsync<bool>(
                         AppSettings.UserManagement.AllowSelfRegistration,
                         tenant.Id))
                {
                    throw new UserFriendlyException(this.L("SelfUserRegistrationIsDisabledMessage_Detail"));
                }

                // Getting tenant-specific settings
                var isNewRegisteredUserActiveByDefault =
                    await
                    this.SettingManager.GetSettingValueForTenantAsync<bool>(
                        AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault,
                        tenant.Id);
                var isEmailConfirmationRequiredForLogin =
                    await
                    this.SettingManager.GetSettingValueForTenantAsync<bool>(
                        AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,
                        tenant.Id);

                var user = new User
                {
                    TenantId = tenant.Id,
                    Name = model.Name,
                    Surname = model.Surname,
                    EmailAddress = model.EmailAddress,
                    IsActive = isNewRegisteredUserActiveByDefault
                };

                ExternalLoginInfo externalLoginInfo = null;
                if (model.IsExternalLogin)
                {
                    externalLoginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync();
                    if (externalLoginInfo == null)
                    {
                        throw new ApplicationException("Can not external login!");
                    }

                    user.Logins = new List<UserLogin>
                                      {
                                          new UserLogin
                                              {
                                                  LoginProvider =
                                                      externalLoginInfo.Login.LoginProvider,
                                                  ProviderKey =
                                                      externalLoginInfo.Login.ProviderKey
                                              }
                                      };

                    model.UserName = model.EmailAddress;
                    model.Password = YUNXI.EDUS.Authorization.Users.User.CreateRandomPassword();

                    if (string.Equals(
                        externalLoginInfo.Email,
                        model.EmailAddress,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        user.IsEmailConfirmed = true;
                    }
                }
                else
                {
                    if (model.UserName.IsNullOrEmpty() || model.Password.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException(this.L("FormIsNotValidMessage"));
                    }
                }

                user.UserName = model.UserName;
                user.Password = new PasswordHasher().HashPassword(model.Password);

                user.Roles = new List<UserRole>();
                foreach (var defaultRole in await this.roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    user.Roles.Add(new UserRole { RoleId = defaultRole.Id });
                }

                this.CheckErrors(await this.userManager.CreateAsync(user));
                await this.unitOfWorkManager.Current.SaveChangesAsync();

                if (!user.IsEmailConfirmed)
                {
                    user.SetNewEmailConfirmationCode();
                    await this.userEmailer.SendEmailActivationLinkAsync(user);
                }

                // Notifications
                await
                    this.notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(
                        user.ToUserIdentifier());
                await this.appNotifier.WelcomeToTheApplicationAsync(user);
                await this.appNotifier.NewUserRegisteredAsync(user);

                // Directly login if possible
                if (user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin))
                {
                    AbpLoginResult<Tenant, User> loginResult;
                    if (externalLoginInfo != null)
                    {
                        loginResult = await this.logInManager.LoginAsync(externalLoginInfo.Login, tenant.TenancyName);
                    }
                    else
                    {
                        loginResult = await this.GetLoginResultAsync(user.UserName, model.Password, tenant.TenancyName);
                    }

                    if (loginResult.Result == AbpLoginResultType.Success)
                    {
                        this.SignInAsync(loginResult);
                        return this.Redirect(this.Url.Action("Index", "Dashboard"));
                    }

                    this.Logger.Warn(
                        "New registered user could not be login. This should not be normally. login result: "
                        + loginResult.Result);
                }

                return this.View(
                    "RegisterResult",
                    new RegisterResultViewModel
                    {
                        TenancyName = tenant.TenancyName,
                        NameAndSurname = user.Name + " " + user.Surname,
                        UserName = user.UserName,
                        EmailAddress = user.EmailAddress,
                        IsActive = user.IsActive,
                        IsEmailConfirmationRequired = isEmailConfirmationRequiredForLogin
                    });
            }
            catch (UserFriendlyException ex)
            {
                this.ViewBag.IsMultiTenancyEnabled = this.multiTenancyConfig.IsEnabled;
                this.ViewBag.UseCaptcha = !model.IsExternalLogin && this.UseCaptchaOnRegistration();
                this.ViewBag.ErrorMessage = ex.Message;

                return this.View("Register", model);
            }
        }

        public ActionResult RegisterView(RegisterViewModel model)
        {
            this.CheckSelfRegistrationIsEnabled();

            this.ViewBag.IsMultiTenancyEnabled = this.multiTenancyConfig.IsEnabled;
            this.ViewBag.UseCaptcha = !model.IsExternalLogin && this.UseCaptchaOnRegistration();

            return this.View("Register", model);
        }

        [UnitOfWork]
        public virtual async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            this.CheckModelState();

            var tenantId = model.TenantId.IsNullOrEmpty()
                               ? (int?)null
                               : SimpleStringCipher.Instance.Decrypt(model.TenantId).To<int>();
            var userId = SimpleStringCipher.Instance.Decrypt(model.UserId).To<long>();

            this.unitOfWorkManager.Current.SetTenantId(tenantId);

            var user = await this.userManager.GetUserByIdAsync(userId);
            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != model.ResetCode)
            {
                throw new UserFriendlyException(
                    this.L("InvalidPasswordResetCode"),
                    this.L("InvalidPasswordResetCode_Detail"));
            }

            return this.View(model);
        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<ActionResult> ResetPassword(ResetPasswordFormViewModel model)
        {
            this.CheckModelState();

            var tenantId = model.TenantId.IsNullOrEmpty()
                               ? (int?)null
                               : SimpleStringCipher.Instance.Decrypt(model.TenantId).To<int>();
            var userId = Convert.ToInt64(SimpleStringCipher.Instance.Decrypt(model.UserId));

            this.unitOfWorkManager.Current.SetTenantId(tenantId);

            var user = await this.userManager.GetUserByIdAsync(userId);
            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != model.ResetCode)
            {
                throw new UserFriendlyException(
                    this.L("InvalidPasswordResetCode"),
                    this.L("InvalidPasswordResetCode_Detail"));
            }

            user.Password = new PasswordHasher().HashPassword(model.Password);
            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;
            user.ShouldChangePasswordOnNextLogin = false;

            await this.userManager.UpdateAsync(user);

            if (user.IsActive)
            {
                await this.SignInAsync(user);
            }

            return this.RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [UnitOfWork]
        [ValidateAntiForgeryToken]
        public virtual async Task<JsonResult> SendEmailActivationLink(SendEmailActivationLinkViewModel model)
        {
            this.CheckModelState();

            var tenantId = await this.GetTenantIdOrDefault(model.TenancyName);

            this.UnitOfWorkManager.Current.SetTenantId(tenantId);

            var user = await this.GetUserByChecking(model.EmailAddress);

            user.SetNewEmailConfirmationCode();
            await this.userEmailer.SendEmailActivationLinkAsync(user);

            return this.Json(new AjaxResponse());
        }

        [HttpPost]
        [UnitOfWork]
        [ValidateAntiForgeryToken]
        public virtual async Task<JsonResult> SendPasswordResetLink(SendPasswordResetLinkViewModel model)
        {
            this.CheckModelState();

            this.UnitOfWorkManager.Current.SetTenantId(await this.GetTenantIdOrDefault(model.TenancyName));

            var user = await this.GetUserByChecking(model.EmailAddress);

            user.SetNewPasswordResetCode();
            await this.userEmailer.SendPasswordResetLinkAsync(user);

            await this.UnitOfWorkManager.Current.SaveChangesAsync();

            return this.Json(new AjaxResponse());
        }

        [UnitOfWork]
        [AbpMvcAuthorize]
        public virtual async Task<JsonResult> SwitchToLinkedAccount(SwitchToLinkedAccountModel model)
        {
            if (
                !await
                 this.userLinkManager.AreUsersLinked(this.AbpSession.ToUserIdentifier(), model.ToUserIdentifier()))
            {
                throw new ApplicationException("This account is not linked to your account");
            }

            var result =
                await
                this.SaveAccountSwitchTokenAndGetTargetUrl(model.TargetTenantId, model.TargetUserId, model.TargetUrl);
            this.authenticationManager.SignOutAll();
            return result;
        }

        [UnitOfWork]
        public virtual async Task<ActionResult> SwitchToLinkedAccountSignIn(string tokenId)
        {
            var cacheItem = await this.cacheManager.GetSwitchToLinkedAccountCache().GetOrDefaultAsync(tokenId);
            if (cacheItem == null)
            {
                throw new UserFriendlyException(this.L("SwitchToLinkedAccountTokenErrorMessage"));
            }

            // Switch to requested tenant
            // this.unitOfWorkManager.Current.SetTenantId(cacheItem.TargetTenantId);

            // Get the user from tenant
            var user = await this.userManager.FindByIdAsync(cacheItem.TargetUserId);

            // Create identity
            var identity =
                await this.userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            // Add claims for audit logging
            if (cacheItem.ImpersonatorTenantId.HasValue)
            {
                identity.AddClaim(
                    new Claim(
                        AbpClaimTypes.ImpersonatorTenantId,
                        cacheItem.ImpersonatorTenantId.Value.ToString(CultureInfo.InvariantCulture)));
            }

            if (cacheItem.ImpersonatorUserId.HasValue)
            {
                identity.AddClaim(
                    new Claim(
                        AbpClaimTypes.ImpersonatorUserId,
                        cacheItem.ImpersonatorUserId.Value.ToString(CultureInfo.InvariantCulture)));
            }

            // Sign in with the target user
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            this.AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

            //user.LastLoginTime = Clock.Now;

            // Remove the cache item to prevent re-use
            await this.cacheManager.GetSwitchToLinkedAccountCache().RemoveAsync(tokenId);

            if (cacheItem.TargetUrl.IsNullOrEmpty())
            {
                return this.RedirectToAction("Index", "Dashboard");
            }

            return this.Redirect(cacheItem.TargetUrl);

            // Json(new AjaxResponse { TargetUrl = cacheItem.TargetUrl },JsonRequestBehavior.AllowGet);
        }

        [AbpMvcAuthorize]
        public async Task<ActionResult> TestNotification(string message = "", string severity = "info")
        {
            if (message.IsNullOrEmpty())
            {
                message = "This is a test notification, created at " + Clock.Now;
            }

            await
                this.appNotifier.SendMessageAsync(
                    this.AbpSession.ToUserIdentifier(),
                    message,
                    severity.ToPascalCase(CultureInfo.InvariantCulture).ToEnum<NotificationSeverity>());

            return this.Content("Sent notification: " + message);
        }

        [UnitOfWork]
        protected virtual async Task<List<Tenant>> FindPossibleTenantsOfUserAsync(UserLoginInfo login)
        {
            List<User> allUsers;

            using (this.unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                allUsers = await this.userManager.FindAllAsync(login);
            }

            return
                allUsers.Where(u => u.TenantId != null)
                    .Select(u => AsyncHelper.RunSync(() => this.tenantManager.FindByIdAsync(u.TenantId.Value)))
                    .ToList();
        }

        private static bool TryExtractNameAndSurnameFromClaims(List<Claim> claims, ref string name, ref string surname)
        {
            string foundName = null;
            string foundSurname = null;

            var givennameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
            if (givennameClaim != null && !givennameClaim.Value.IsNullOrEmpty())
            {
                foundName = givennameClaim.Value;
            }

            var surnameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
            if (surnameClaim != null && !surnameClaim.Value.IsNullOrEmpty())
            {
                foundSurname = surnameClaim.Value;
            }

            if (foundName == null || foundSurname == null)
            {
                var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (nameClaim != null)
                {
                    var nameSurName = nameClaim.Value;
                    if (!nameSurName.IsNullOrEmpty())
                    {
                        var lastSpaceIndex = nameSurName.LastIndexOf(' ');
                        if (lastSpaceIndex < 1 || lastSpaceIndex > nameSurName.Length - 2)
                        {
                            foundName = foundSurname = nameSurName;
                        }
                        else
                        {
                            foundName = nameSurName.Substring(0, lastSpaceIndex);
                            foundSurname = nameSurName.Substring(lastSpaceIndex);
                        }
                    }
                }
            }

            if (!foundName.IsNullOrEmpty())
            {
                name = foundName;
            }

            if (!foundSurname.IsNullOrEmpty())
            {
                surname = foundSurname;
            }

            return foundName != null && foundSurname != null;
        }

        private void CheckSelfRegistrationIsEnabled()
        {
            if (!this.IsSelfRegistrationEnabled())
            {
                throw new UserFriendlyException(this.L("SelfUserRegistrationIsDisabledMessage_Detail"));
            }
        }

        private async Task<Tenant> GetActiveTenantAsync(string tenancyName)
        {
            var tenant = await this.tenantManager.FindByTenancyNameAsync(tenancyName);
            if (tenant == null)
            {
                throw new UserFriendlyException(this.L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(this.L("TenantIsNotActive", tenancyName));
            }

            return tenant;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(
            string usernameOrEmailAddress,
            string password,
            string tenancyName)
        {
            // var loginResult = await _userManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);
            var loginResult = await this.logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw this.abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                        loginResult.Result,
                        usernameOrEmailAddress,
                        tenancyName);
            }
        }

        private async Task<int?> GetTenantIdOrDefault(string tenancyName)
        {
            return tenancyName.IsNullOrEmpty()
                       ? this.AbpSession.TenantId
                       : (await this.GetActiveTenantAsync(tenancyName)).Id;
        }

        private async Task<User> GetUserByChecking(string emailAddress)
        {
            var user = await this.userManager.Users.Where(u => u.EmailAddress == emailAddress).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new UserFriendlyException(this.L("InvalidEmailAddress"));
            }

            return user;
        }

        private bool IsSelfRegistrationEnabled()
        {
            var tenancyName = this.tenancyNameFinder.GetCurrentTenancyNameOrNull();
            if (tenancyName.IsNullOrEmpty())
            {
                return true;
            }

            var tenant = AsyncHelper.RunSync(() => this.GetActiveTenantAsync(tenancyName));
            return this.SettingManager.GetSettingValueForTenant<bool>(
                AppSettings.UserManagement.AllowSelfRegistration,
                tenant.Id);
        }

        private async Task<ActionResult> RegisterView(ExternalLoginInfo loginInfo, string tenancyName = null)
        {
            var name = loginInfo.DefaultUserName;
            var surname = loginInfo.DefaultUserName;

            var extractedNameAndSurname = TryExtractNameAndSurnameFromClaims(
                loginInfo.ExternalIdentity.Claims.ToList(),
                ref name,
                ref surname);

            var viewModel = new RegisterViewModel
            {
                TenancyName = tenancyName,
                EmailAddress = loginInfo.Email,
                Name = name,
                Surname = surname,
                IsExternalLogin = true
            };

            if (!tenancyName.IsNullOrEmpty() && extractedNameAndSurname)
            {
                return await this.Register(viewModel);
            }

            return this.RegisterView(viewModel);
        }

        /// <summary>
        ///     保存账户切换的Token 和 TargetUrl
        /// </summary>
        /// <param name="targetTenantId"></param>
        /// <param name="targetUserId"></param>
        /// <param name="oldTargetUrl"></param>
        /// <returns></returns>
        private async Task<JsonResult> SaveAccountSwitchTokenAndGetTargetUrl(
            int? targetTenantId,
            long targetUserId,
            string oldTargetUrl)
        {
            // Create a cache item
            var cacheItem = new SwitchToLinkedAccountCacheItem(
                targetTenantId,
                targetUserId,
                this.AbpSession.ImpersonatorTenantId,
                this.AbpSession.ImpersonatorUserId,
                oldTargetUrl);

            // Create a random token and save to the cache
            var tokenId = Guid.NewGuid().ToString();
            await
                this.cacheManager.GetSwitchToLinkedAccountCache().SetAsync(tokenId, cacheItem, TimeSpan.FromMinutes(1));

            // Find tenancy name
            string tenancyName = null;
            if (targetTenantId.HasValue)
            {
                tenancyName = (await this.tenantManager.GetByIdAsync(targetTenantId.Value)).TenancyName;
            }

            // Create target URL
            var targetUrl = this.webUrlService.GetSiteRootAddress(tenancyName)
                            + "Account/SwitchToLinkedAccountSignIn?tokenId=" + tokenId;

            return this.Json(new AjaxResponse { TargetUrl = targetUrl });
        }

        private async Task<JsonResult> SaveImpersonationTokenAndGetTargetUrl(
            int? tenantId,
            long userId,
            bool isBackToImpersonator)
        {
            // Create a cache item
            var cacheItem = new ImpersonationCacheItem(tenantId, userId, isBackToImpersonator);

            if (!isBackToImpersonator)
            {
                cacheItem.ImpersonatorTenantId = this.AbpSession.TenantId;
                cacheItem.ImpersonatorUserId = this.AbpSession.GetUserId();
            }

            // Create a random token and save to the cache
            var tokenId = Guid.NewGuid().ToString();
            await this.cacheManager.GetImpersonationCache().SetAsync(tokenId, cacheItem, TimeSpan.FromMinutes(1));

            // Find tenancy name
            string tenancyName = null;
            if (tenantId.HasValue)
            {
                tenancyName = (await this.tenantManager.GetByIdAsync(tenantId.Value)).TenancyName;
            }

            // Create target URL
            var targetUrl = this.webUrlService.GetSiteRootAddress(tenancyName) + "Account/ImpersonateSignIn?tokenId="
                            + tokenId;
            return this.Json(new AjaxResponse { TargetUrl = targetUrl });
        }

        private async Task SignInAsync(User user, bool rememberMe = false)
        {
            var identity =
                await this.userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            this.AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe }, identity);
        }

        private void SignInAsync(AbpLoginResult<Tenant, User> loginResult, bool rememberMe = false)
        {
            // 登记租户信息
            var tenancySides = MultiTenancySides.Host;
            loginResult.Identity.AddClaim(new Claim(WimisoftClaims.MultiTenancySides, tenancySides.ToString()));

            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            this.AuthenticationManager.SignIn(
                new AuthenticationProperties { IsPersistent = rememberMe },
                loginResult.Identity);
        }

        private bool UseCaptchaOnRegistration()
        {
            if (DebugHelper.IsDebug)
            {
                return false;
            }

            var tenancyName = this.tenancyNameFinder.GetCurrentTenancyNameOrNull();
            if (tenancyName.IsNullOrEmpty())
            {
                return true;
            }

            var tenant = AsyncHelper.RunSync(() => this.GetActiveTenantAsync(tenancyName));
            return
                this.SettingManager.GetSettingValueForTenant<bool>(
                    AppSettings.UserManagement.UseCaptchaOnRegistration,
                    tenant.Id);
        }

        private static class WimisoftClaims
        {
            /// <summary>
            ///     MultiTenancySides
            /// </summary>
            public const string MultiTenancySides = "http://cloud.wimisoft.com";
        }

        private class VisionEditorLoginResult
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Message { get; set; }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Status { get; set; }
        }
    }
}