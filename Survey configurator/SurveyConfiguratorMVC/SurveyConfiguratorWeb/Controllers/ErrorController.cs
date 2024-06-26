using SharedResources;
using SurveyConfiguratorWeb.Filters;
using System;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Controllers
{
    [GlobalExceptionFilter]
    public class ErrorController : Controller
    {
        /// <summary>
        /// Error controller to return an error view
        /// related to the occuring error
        /// </summary>




        /// <summary>
        /// returns an error view desciribing the occured error
        /// </summary>
        /// <param name="pErrorMessage">Error message to show on the page</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ErrorPage(string pErrorMessage)
        {
            try 
            { 
                ViewBag.ErrorMessage = pErrorMessage;
                return View();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction("ErrorPage", "Error", new { ErrorMessage = GlobalStrings.PageLoadingError });
            }
        }
    }
}