using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Hypnofrog.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Twitter;

namespace Hypnofrog
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Register"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            app.UseTwitterAuthentication(
         new TwitterAuthenticationOptions
         {
             ConsumerKey = "CzBCZOFBR8iL7M5x2NcZuhXlK",
             ConsumerSecret = "TTsws88H0dM7Y3p2u6ruRiYQcI6rQs5PrMnOHhfAsfNAcswGfv",
             BackchannelCertificateValidator = new CertificateSubjectKeyIdentifierValidator(
            new[]
            {
              "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
               "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
               "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
               "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
               "4EB6D578499B1CCF5F581EAD56BE3D9B6744A5E5", // VeriSign Class 3 Primary CA - G5
               "5168FF90AF0207753CCCD9656462A212B859723B", // DigiCert SHA2 High Assurance Server C‎A 
               "B13EC36903F8BF4701D498261A0802EF63642BC3", // DigiCert High Assurance EV Root CA
               "B77DDB6867D3B325E01C90793413E15BF0E44DF2"
            })
         });

            app.UseFacebookAuthentication(
               appId: "1564846623818431",
               appSecret: "ce6db174679bb070bdb6eb872bdabc31");

            app.UseVkontakteAuthentication(
                appId: "5565888",
                appSecret: "DY6yvOKLrwTUxwq01Ay5",
                scope:"email,");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}