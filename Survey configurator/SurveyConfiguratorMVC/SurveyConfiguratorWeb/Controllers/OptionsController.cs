using QuestionServices;
using SharedResources;
using SurveyConfiguratorWeb.ConstantsAndMethods;
using SurveyConfiguratorWeb.Models;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
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

        //array of supported languages 
        public static string[] cSupportedLanguages = { "en", "ar" };


        /// <summary>
        /// shows the options page
        /// </summary>
        /// <returns>Options view</returns>
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController, new { ErrorMessage = GlobalStrings.UnknownError });
            }
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
            try
            {
                //serialize the supported language array to be able to use it in the view
                ViewBag.SupportedLanguages = JsonSerializer.Serialize(cSupportedLanguages);
                return View();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController, new { ErrorMessage = GlobalStrings.UnknownError });

            }
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
            try
            {
                //extract the selected language value from the form data
                string tSelectedLanguage = pFormData[SharedConstants.cLanguageDropDownId];

                //check if the received value exists in the supported Languages
                if (!cSupportedLanguages.Contains(tSelectedLanguage))
                {
                    TempData[SharedConstants.cMessageKey] = GlobalStrings.UnSupportedLanguageError;
                    return RedirectToAction(SharedConstants.cOptionsIndexAction);
                }

                //set the selected language as default for all threads
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(tSelectedLanguage);
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(tSelectedLanguage);

                //save to app config, so when visiting the website the next time language options will be saved.
                Configuration tConfigObject = WebConfigurationManager.OpenWebConfiguration("~");
                AppSettingsSection tAppSettingsObj = (AppSettingsSection)tConfigObject.GetSection("appSettings");
                if (tAppSettingsObj != null)
                {
                    tAppSettingsObj.Settings[SharedConstants.cLagnaugeAppSettingKey].Value = tSelectedLanguage;
                    tConfigObject.Save();
                }
                return RedirectToAction(SharedConstants.cOptionsIndexAction);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController, new { ErrorMessage = GlobalStrings.UnknownError });
            }
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
            try
            {
                //try to get existing connection string if it's existing
                ConnectionString tConnectionSettings = SharedData.mConnectionString;
                if(tConnectionSettings !=  null) { 
                    //encapsulate the connection settings to a connection settings view model
                    ConnectionStringViewModel tConnectionSettingsModel = new ConnectionStringViewModel(
                        tConnectionSettings.mServer,
                        tConnectionSettings.mDatabase,
                        tConnectionSettings.mIntegratedSecurity,
                        tConnectionSettings.mUser,
                        tConnectionSettings.mPassword
                        );
                    //return connection settings view with existing data from the connectino string
                    return View(tConnectionSettingsModel);
                }
                //return connection settings view without data from connection string
                return View(new ConnectionStringViewModel());
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController, new { ErrorMessage = GlobalStrings.UnknownError });
            }
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
            try
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
                    //more enhancements to visuals 

                    ViewData[SharedConstants.cConnectionResultMessageKey] = SharedConstants.cConnectionSuccessfulMessage;
                    return View(pConnectionSettings);
                }
                else
                {
                    //more enhancements to visuals

                    ViewData[SharedConstants.cConnectionResultMessageKey] = SharedConstants.cConnectionFailedMessage;
                    return View(pConnectionSettings);
                }
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController, new { ErrorMessage = GlobalStrings.UnknownError });
            }
        }

    }
}