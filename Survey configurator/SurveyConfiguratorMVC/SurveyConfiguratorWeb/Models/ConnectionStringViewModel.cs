using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SurveyConfiguratorWeb.Models
{
    public class ConnectionStringViewModel
    {
        /// <summary>
        /// view model for the Connection string object
        /// with the appropriate data annotations to help 
        /// with the validation
        /// the errors messages are fetched from the resource file to 
        /// enable the localization 
        /// </summary>

        [Required(ErrorMessage ="Server name is requried")]
        [DisplayName("Server")]
        public string mServer { get; set; }

        [Required(ErrorMessage ="Database name is required")]
        [DisplayName("Database")]
        public string mDatabase { get; set; }

        [Required(ErrorMessage ="User name is required")]
        [DisplayName("User")]
        public string mUser { get; set; }

        [Required(ErrorMessage ="Password is requried")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string mPassword { get; set; }

        [Required(ErrorMessage ="Integrated security option is required")]
        [DisplayName("Authentication")]
        public bool mIntegratedSecurity { get; set; }

        public ConnectionStringViewModel()
        {
        }

        public ConnectionStringViewModel(string pServer, string pDatabase, bool pIntegratedSecurity, string pUser="", string pPassword="")
        {
            this.mServer = pServer;
            this.mDatabase = pDatabase;
            this.mIntegratedSecurity = pIntegratedSecurity;
            this.mUser = pUser;
            this.mPassword = pPassword;
        }
    }
}