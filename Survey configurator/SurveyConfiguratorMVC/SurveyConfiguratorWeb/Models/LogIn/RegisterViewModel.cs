using SharedResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SurveyConfiguratorWeb.Models.LogIn
{
    public class RegisterViewModel
    {
        private const int cUserNameMinLength = 6;
        private const int cPasswordMinLength = 6;

        [Required(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "UserNameRequiredError")]
        [MinLength(cUserNameMinLength, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "UserNameLengthError")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "EmailRequiredError")]
        [EmailAddress(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "EmailFormatError")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "PasswordRequiredError")]
        [MinLength(cPasswordMinLength, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "PasswordLengthError")]
        //change the pattern later
        [RegularExpression("dsalfj", ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "PasswordFormatError")]
        public string Password { get; set; }
    }
}