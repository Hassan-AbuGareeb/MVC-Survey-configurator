using SurveyConfiguratorWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Controllers
{
    public class OptionsController : Controller
    {
        public static string[] cSupportedLanguages = { "en", "ar" };

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Language()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Language(FormCollection pFormData)
        {
            //check if the received value is existing in the supportedLanguages

            //set the selected language as default for all threads
            string tSelectedLanguage = pFormData["SelectedLanguage"];
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
            return View();
        }

        [HttpPost]
        public ActionResult ConnectionSettings(ConnectionStringViewModel pConnectionSettings, FormCollection pFormData)
        {
            return View("Index");
        }
    }
}