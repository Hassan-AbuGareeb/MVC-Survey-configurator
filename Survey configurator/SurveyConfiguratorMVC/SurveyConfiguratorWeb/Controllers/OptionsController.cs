using QuestionServices;
using SharedResources;
using SurveyConfiguratorWeb.ConstantsAndMethods;
using SurveyConfiguratorWeb.Filters;
using SurveyConfiguratorWeb.Models;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Controllers
{
    [GlobalExceptionFilter]
    public class OptionsController : Controller
    {
        /// <summary>
        /// responsible for the options view which contains
        /// options for the language as to change it, and options
        /// for the connection string settings.
        /// </summary>


        /// <summary>
        /// shows the connection string settings page
        /// and fills the fields with the connection
        /// string data if it exists
        /// </summary>
        /// <returns> connection string settings view </returns>
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                //try to get existing connection string if it's existing
                ConnectionString tConnectionSettings = SharedData.mConnectionString;
                if (tConnectionSettings != null)
                {
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
        /// to communicate with the database
        /// </summary>
        /// <param name="pConnectionSettings">connection string settings view model object</param>
        /// <returns>view to be redirected to</returns>
        [HttpPost]
        public ActionResult Save(ConnectionStringViewModel pConnectionSettings)
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
                TempData[SharedConstants.cMessageKey] = GlobalStrings.ChangesSaved;
                return View(SharedConstants.cOptionsIndexAction, pConnectionSettings);

                
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController, new { ErrorMessage = GlobalStrings.UnknownError });
            }
        }

        /// <summary>
        /// test the connection to the database using the entered
        /// connection settings without acutally saving them
        /// </summary>
        /// <param name="pConnectionSettings"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Test(ConnectionStringViewModel pConnectionSettings)
        {
            try
            {
                //store the already existing connection string settings
                ConnectionString tOriginalConnectionSettings = SharedData.mConnectionString;

                //create connectionString object form user input
                ConnectionString tNewConnectionSettings = new ConnectionString(
                    pConnectionSettings.mServer,
                    pConnectionSettings.mDatabase,
                    pConnectionSettings.mIntegratedSecurity,
                    pConnectionSettings.mUser,
                    pConnectionSettings.mPassword
                    );

                //save the obtained connection settings to use them to test connection
                QuestionOperations.SetConnectionString(tNewConnectionSettings);

                //test connection, return notificiation result based on whether connection succeeded or not
                OperationResult tCanConnectToDatabase = QuestionOperations.TestDBConnection();
                if (tCanConnectToDatabase.IsSuccess)
                {
                    //more enhancements to visuals 
                    ViewData[SharedConstants.cConnectionResultMessageKey] = SharedConstants.cConnectionSuccessfulMessage;
                }
                else
                {
                    //more enhancements to visuals
                    ViewData[SharedConstants.cConnectionResultMessageKey] = SharedConstants.cConnectionFailedMessage;
                }

                //restore original connection string settings
                QuestionOperations.SetConnectionString(tOriginalConnectionSettings);

                return View(SharedConstants.cOptionsIndexAction, pConnectionSettings);

            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController, new { ErrorMessage = GlobalStrings.UnknownError });
            }
        }

        /// <summary>
        /// sets the received language as the default language for all
        /// threads to change the apps language, change the state representing
        /// the current app language and saves the new language to the
        /// web.config file to persist when the app is accessed again,
        /// the saving process is done on a seperate thread as to not slow
        /// down the application.
        /// </summary>
        /// <param name="pSelectedLanguage">the selected language symbol</param>
        /// <param name="pOrigianlUrl">the url of the page that the user was already in</param>
        /// <returns>view to be redirected to</returns>
        public ActionResult Language(string pSelectedLanguage, string pOrigianlUrl)
        {
            try
            {
                //check if the received value exists in the supported Languages
                if (!SharedConstants.cSupportedLanguages.Contains(pSelectedLanguage))
                {
                    TempData[SharedConstants.cMessageKey] = GlobalStrings.UnSupportedLanguageError;
                    return RedirectToAction(SharedConstants.cQuestionsIndexAction, SharedConstants.cQuestionsController);
                }

                //change the state of the current app language
                States.CurrentAppLanguage = pSelectedLanguage;

                //set the selected language as default for all threads
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(States.CurrentAppLanguage);
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(States.CurrentAppLanguage);

                //create a thread to save the app settings
                Thread tSaveNewLanguage = new Thread(() => UpdateAppSettings());
                tSaveNewLanguage.Start();

                //redirect to the home page
                return Redirect(pOrigianlUrl);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController, new { ErrorMessage = GlobalStrings.UnknownError });
            }
        }


        #region class utility functions
        private static void UpdateAppSettings()
        {
            //save to app config, so when visiting the website the next time language options will be saved.
            Configuration tConfigObject = WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection tAppSettingsObj = (AppSettingsSection)tConfigObject.GetSection("appSettings");
            if (tAppSettingsObj != null)
            {
                tAppSettingsObj.Settings[SharedConstants.cLagnaugeAppSettingKey].Value = States.CurrentAppLanguage;
                tConfigObject.Save();
            }
        }


        #endregion
    }
}