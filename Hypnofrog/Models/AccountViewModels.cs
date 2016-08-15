using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Strings;

namespace Hypnofrog.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(ResourceType = typeof(Creditals), Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(ResourceType = typeof(Creditals), Name = "Name")]
        public string Name { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(ResourceType = typeof(Creditals), Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(ResourceType = typeof(Creditals), Name = "Remember_this_browser")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(ResourceType = typeof(Creditals), Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(ResourceType = typeof(Creditals), Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Creditals), Name = "Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Creditals), Name = "Remember_me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(ResourceType = typeof(Creditals), Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(20, ErrorMessageResourceType = typeof(Creditals), ErrorMessageResourceName = "RegisterViewModel_Name_rules", MinimumLength = 5)]
        [Display(ResourceType = typeof(Creditals), Name = "Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(Creditals), ErrorMessageResourceName = "RegisterViewModel_Name_rules", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Creditals), Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Creditals), Name = "Confirm_password")]
        [Compare("Password", ErrorMessageResourceType = typeof(Creditals), ErrorMessageResourceName = "Do_not_match")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(ResourceType = typeof(Creditals), Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(Creditals), ErrorMessageResourceName = "RegisterViewModel_Name_rules", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Creditals), Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Creditals), Name = "Confirm_password")]
        [Compare("Password", ErrorMessageResourceType = typeof(Creditals), ErrorMessageResourceName = "Do_not_match")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(ResourceType = typeof(Creditals), Name = "Email")]
        public string Email { get; set; }
    }
}
