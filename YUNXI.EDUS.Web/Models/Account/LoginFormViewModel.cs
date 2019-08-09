namespace YUNXI.EDUS.Web.Models.Account
{
    public class LoginFormViewModel
    {
        public bool IsSelfRegistrationEnabled { get; set; }

        public string SuccessMessage { get; set; }

        public string TenancyName { get; set; }

        public string UserNameOrEmailAddress { get; set; }

        public LanguagesViewModel LanguageViewModel { get; set; }
    }
}