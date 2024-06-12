using SharedResources;
using SurveyConfiguratorWeb.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Controllers
{
    [GlobalExceptionFilter]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult ErrorPage(string ErrorMessage)
        {
            try 
            { 
                ViewBag.ErrorMessage = ErrorMessage;
                return View();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction("ErrorPage", "Error", new { ErrorMessage = "Error occured while Loading your page, please try again" });
            }

        }
    }
}