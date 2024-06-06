using QuestionServices;
using SharedResources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Questions()
        {
            var canGetQuesitons = QuestionOperations.GetQuestions();
            if (canGetQuesitons.IsSuccess)
            {
                var model = QuestionOperations.mQuestionsList;
                return View(model);
            }
            return View(new List<Question>());
        }

    }
}