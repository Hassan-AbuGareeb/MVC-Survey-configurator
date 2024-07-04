using SharedResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SurveyConfiguratorWeb.Models
{
    public class LogInViewModel
    {
        [Required(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "UserNameRequiredError")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "PasswordRequiredError")]
        public string Password { get; set; }
    }
}