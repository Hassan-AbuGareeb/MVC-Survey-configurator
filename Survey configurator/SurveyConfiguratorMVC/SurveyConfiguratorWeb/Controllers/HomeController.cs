using QuestionServices;
using SharedResources;
using SharedResources.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
            //Initialize the questions List and do the 
            //stuff that was done in the winforms load method
        }

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
            //handle case of failure to obtain questions
            return View(new List<Question>());
        }

        [HttpGet]
        public ActionResult Delete(int PQuestionId)
        {
            try 
            { 
                var tQuestionData = QuestionOperations.GetQuestionData(PQuestionId);
                if(tQuestionData != null)
                {
                    return View(tQuestionData);
                }
                //handle the case of question not found
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                //log error and show error page
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Question pQuestionData)
        {
            try
            {
                List<int> tQuestionsIds = new List<int>();
                tQuestionsIds.Add(pQuestionData.Id);
                OperationResult tAreQuestionsDeleted = QuestionOperations.DeleteQuestion(tQuestionsIds);
                if (tAreQuestionsDeleted.IsSuccess)
                {
                    return RedirectToAction("Questions");
                }
                else
                {
                    //show error in deletion
                    return RedirectToAction("Questions");
                }
            }
            catch (Exception ex)
            {
                //log error and show error page
                return RedirectToAction("Index");
            }
        }
    }
}