using QuestionServices;
using SharedResources;
using SurveyConfiguratorWeb.Models;
using System.Configuration;
using System.Globalization;
using System.Text.Json;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Controllers
{
    public class OptionsController : Controller
    {
        /// <summary>
        /// responsible for the options view which contains
        /// options for the language as to change it, and options
        /// for the connection string settings.
        /// </summary>


        public static string[] cSupportedLanguages = { "en", "ar" };

        //constants
        static string cConnectionResultMessageKey = "ConnectionResult";

        /// <summary>
        /// shows the options page
        /// </summary>
        /// <returns>Options view</returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// shows the language options page, and sends
        /// the supported language array after serializing it
        /// to the view as to populate the drop down list
        /// containing the choices for the language
        /// </summary>
        /// <returns>language options view</returns>
        [HttpGet]
        public ActionResult Language()
        {
            //serialize the supported language array to be able to use it in the view
            ViewBag.SupportedLanguages = JsonSerializer.Serialize(cSupportedLanguages);
            return View();
        }

        /// <summary>
        /// sets the received language as the default language for all
        /// threads to change the apps language, and saves it to the
        /// web.config file to persist when the app is accessed again
        /// </summary>
        /// <param name="pFormData">the submitted form data</param>
        /// <returns>view to be redirected to</returns>
        [HttpPost]
        public ActionResult Language(FormCollection pFormData)
        {
            //check if the received value is existing in the supportedLanguages

            //set the selected language as default for all threads
            string tSelectedLanguage = pFormData["LanguageDropDown"];
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(tSelectedLanguage);
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(tSelectedLanguage);

            //save to app config, so when visiting the website the next time language options will be saved.
            Configuration tConfigObject = WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection tAppSettingsObj = (AppSettingsSection)tConfigObject.GetSection("appSettings");
            if(tAppSettingsObj != null)
            {
                tAppSettingsObj.Settings["Language"].Value = tSelectedLanguage;
                tConfigObject.Save();
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// shows the connection string settings page
        /// and fills the fields with the connection
        /// string data if it's existing
        /// </summary>
        /// <returns> connection string settings view </returns>
        [HttpGet]
        public ActionResult ConnectionSettings()
        {
            //try to get existing connection string if it's existing
            return View();
        }

        /// <summary>
        /// creates a connection string object from the connection
        /// string view model and save it to the connectionSettings.txt file
        /// and sets it as the connection string to be used in the data base layer
        /// shows notification on whether the connection to the database can be 
        /// successfully established
        /// </summary>
        /// <param name="pConnectionSettings">connection string settings view model object</param>
        /// <returns>view to be redirected to</returns>
        [HttpPost]
        public ActionResult ConnectionSettings(ConnectionStringViewModel pConnectionSettings)
        {
            //create connectionString object
            ConnectionString tConnectionData = new ConnectionString(
                pConnectionSettings.mServer,
                pConnectionSettings.mDatabase,
                pConnectionSettings.mIntegratedSecurity,
                pConnectionSettings.mUser,
                pConnectionSettings.mPassword
                );

            //set it as the default connection string 
            QuestionOperations.SetConnectionString(tConnectionData);

            //test connection, return notificiation result based on whether connection succeeded or not
            OperationResult tCanConnectToDatabase = QuestionOperations.TestDBConnection();
            if (tCanConnectToDatabase.IsSuccess)
            {
                //more enhancements and better redirection required

                TempData[cConnectionResultMessageKey] = "Database connected successfully";
                return View();
            }
            else 
            {
                //more enhancements and better redirection required

                TempData[cConnectionResultMessageKey] = "Database refused to connect";
                return View();
            }
        }

    }
}