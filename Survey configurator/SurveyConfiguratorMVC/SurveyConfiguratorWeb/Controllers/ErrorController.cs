using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult ErrorPage(string ErrorMessage)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            return View();
        }
    }
}