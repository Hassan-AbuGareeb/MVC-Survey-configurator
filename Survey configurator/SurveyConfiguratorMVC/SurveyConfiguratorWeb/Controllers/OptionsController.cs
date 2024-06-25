using QuestionServices;
using SharedResources;
using SurveyConfiguratorWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Controllers
{
    public class OptionsController : Controller
    {
        public static string[] cSupportedLanguages = { "en", "ar" };

        //constants
        static string cConnectionResultMessageKey = "ConnectionResult";

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Language()
        {
            ViewBag.SupportedLanguages = JsonSerializer.Serialize(cSupportedLanguages);
            return View();
        }

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

        [HttpGet]
        public ActionResult ConnectionSettings()
        {
            //try to get existing connection string if it's existing
            return View();
        }

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

        //public string[] GetSupportedLanguages()
        //{
        //    return cSupportedLanguages;
        //}
    }
}