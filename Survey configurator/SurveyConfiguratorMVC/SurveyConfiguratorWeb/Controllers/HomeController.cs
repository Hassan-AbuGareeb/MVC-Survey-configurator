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

        //constants
        private const string cQuestionsView = "Questions";

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
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                //log error
                return RedirectToAction(cQuestionsView);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Question pQuestionData, FormCollection data)
        {
            try
            {
                StarsQuestion hello = new StarsQuestion(pQuestionData, Convert.ToInt32(data["NumberOfStars"]));
                OperationResult tIsQuestionAdded = QuestionOperations.AddQuestion(hello);
                if (tIsQuestionAdded.IsSuccess)
                {
                    //on a successful question creation
                    return RedirectToAction(cQuestionsView);
                }
                //show error notfication or error page
                return RedirectToAction(cQuestionsView);
            }
            catch(Exception ex)
            {
                //log err
                return RedirectToAction(cQuestionsView);
            }
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
                    return RedirectToAction(cQuestionsView);
                }
                else
                {
                    //show error in deletion
                    return RedirectToAction(cQuestionsView);
                }
            }
            catch (Exception ex)
            {
                //log error and show error page
                return RedirectToAction("Index");
            }
        }

        public ActionResult GetQuetsionTypeOptions()
        {
            //return view based on switch decision
            return PartialView("_StarsQuestionOptions");
        }
    }
}