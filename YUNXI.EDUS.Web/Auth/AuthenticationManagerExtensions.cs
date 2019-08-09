using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;

namespace YUNXI.EDUS.Web.Auth
{
    public static  class AuthenticationManagerExtensions
    {
        public static void SignOutAll(this IAuthenticationManager authenticationManager)
        {
            authenticationManager.SignOut(
                DefaultAuthenticationTypes.ApplicationCookie,
                DefaultAuthenticationTypes.ExternalCookie,
                DefaultAuthenticationTypes.TwoFactorCookie);
        }

        public static void SignOutAllAndSignIn(
            this IAuthenticationManager authenticationManager,
            ClaimsIdentity identity,
            bool rememberMe = false)
        {
            authenticationManager.SignOutAll();
            authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe }, identity);
        }
    }
}