using QuestionServices;
using SharedResources;
using SharedResources.Models;
using SurveyConfiguratorWeb.Models;
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
        private const string cStarsOptionsView = "_StarsQuestionOptions";
        private const string cSmileyOptionsView = "_SmileyQuestionOptions";
        private const string cSliderOptionsView = "_SliderQuestionOptions";
        private const string cStarsOptionsDetailsView = "_StarsQuestionDetails";
        private const string cSmileyOptionsDetailsView = "_SmileyQuestionDetails";
        private const string cSliderOptionsDetailsView = "_SliderQuestionDetails";
        //stars question properties
        const string cNumberOfStars = "NumberOfStars";
        //Smiley table
        private const string cNumberOfFaces = "NumberOfSmileyFaces";
        //Slider table
        private const string cStartValue = "StartValue";
        private const string cEndValue = "EndValue";
        private const string cStartValueCaption = "StartValueCaption";
        private const string cEndValueCaption = "EndValueCaption";


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
        public ActionResult Create(Question pQuestionData, FormCollection pFormData)
        {
            try
            {
                //based on the type of question create a new object and
                //fill its respective fields

                Question tQuestionToAdd = CreateQuestionObject(pQuestionData, pFormData);

                OperationResult tIsQuestionAdded = QuestionOperations.AddQuestion(tQuestionToAdd);
                if (tIsQuestionAdded.IsSuccess)
                {
                    //on a successful question creation
                    return RedirectToAction(cQuestionsView);
                }
                //show error notfication or error page
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                //log err
                return RedirectToAction(cQuestionsView);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                Question tQuestionData = QuestionOperations.GetQuestionData(id);
                if (tQuestionData != null)
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

        [HttpGet]
        public ActionResult Details(int id)
        {
            try
            {
                Question tQuestionData = QuestionOperations.GetQuestionData(id);
                if (tQuestionData != null)
                {
                    return View(tQuestionData);
                }
                //handle the case of question not found
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                //log error
                return RedirectToAction("Questions");
            }
        }


        #region class utility functions
        [HttpGet]
        public ActionResult GetQuestionTypeOptions(int pType)
        {
            eQuestionType tQuestionType = (eQuestionType)pType;
            string tOptionsViewType = "";
            switch (tQuestionType)
            {
                case eQuestionType.Stars:
                    tOptionsViewType = cStarsOptionsView;
                    break;
                case eQuestionType.Smiley:
                    tOptionsViewType = cSmileyOptionsView;
                    break;
                case eQuestionType.Slider:
                    tOptionsViewType = cSliderOptionsView;
                    break;
            }
            //return view based on switch decision
            return PartialView(tOptionsViewType);
        }

        [HttpGet]
        public ActionResult GetQuestionTypeDetails(int id)
        {
            try { 
                Question tQuestionTypeData = null;
                OperationResult tCanGetQuestionTypeData = QuestionOperations.GetQuestionSpecificData(id, ref tQuestionTypeData);
                if (tCanGetQuestionTypeData.IsSuccess)
                {
                    //return the required partial view filled with data based on question type
                    return GetQuestionTypeDetailsPartialView(tQuestionTypeData);
                }
                //handle failure case
                return RedirectToAction("Questions");
            }
            catch(Exception ex)
            {
                //log error
                return RedirectToAction("Questions");
            }
        }

        private Question CreateQuestionObject(Question pQuestionData, FormCollection pFormData)
        {
            Question tCreatedQuestion = null;
            switch (pQuestionData.Type)
            {
                case eQuestionType.Stars:
                    tCreatedQuestion = new StarsQuestion(pQuestionData, Convert.ToInt32(pFormData[cNumberOfStars]));
                    break;
                case eQuestionType.Smiley:
                    tCreatedQuestion = new SmileyQuestion(pQuestionData, Convert.ToInt32(pFormData[cNumberOfFaces]));
                    break;
                case eQuestionType.Slider:
                    tCreatedQuestion = new SliderQuestion
                        (
                        pQuestionData,
                        Convert.ToInt32(pFormData[cStartValue]),
                        Convert.ToInt32(pFormData[cEndValue]),
                        pFormData[cStartValueCaption],
                        pFormData[cEndValueCaption]
                        );
                    break;
            }
            return tCreatedQuestion;
        }

        private ActionResult GetQuestionTypeDetailsPartialView(Question pQuestionData)
        {
            switch(pQuestionData.Type)
            {
                case eQuestionType.Stars:
                    StarsQuestion tStarsQuestionData = (StarsQuestion)pQuestionData;
                    return PartialView(cStarsOptionsDetailsView, new StarsQuestionOptions(tStarsQuestionData.NumberOfStars));
                case eQuestionType.Smiley:
                    SmileyQuestion tSmileyQuestionData = (SmileyQuestion)pQuestionData;
                    return PartialView(cSmileyOptionsDetailsView, new SmileyQuestionOptions(tSmileyQuestionData.NumberOfSmileyFaces));
                case eQuestionType.Slider:
                    SliderQuestion tSliderQuestionData = (SliderQuestion)pQuestionData;
                    return PartialView(cSliderOptionsDetailsView, new SliderQuestionOptions(
                        tSliderQuestionData.StartValue,
                        tSliderQuestionData.EndValue,
                        tSliderQuestionData.StartValueCaption,
                        tSliderQuestionData.EndValueCaption
                        ));
            }
            return null;
        }

        #endregion
    }
}