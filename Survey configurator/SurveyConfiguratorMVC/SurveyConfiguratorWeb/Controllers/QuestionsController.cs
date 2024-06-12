using QuestionServices;
using SharedResources;
using SharedResources.Models;
using SurveyConfiguratorWeb.Filters;
using SurveyConfiguratorWeb.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
namespace SurveyConfiguratorWeb.Controllers
{
    [GlobalExceptionFilter]
    public class QuestionsController : Controller
    {
        //constants
        private const string cQuestionsView = "Index";
        private const string cPartialViewsFolder = "PartialViews";
        private const string cStarsOptionsView = cPartialViewsFolder + "/_StarsQuestionOptions";
        private const string cSmileyOptionsView = cPartialViewsFolder + "/_SmileyQuestionOptions";
        private const string cSliderOptionsView = cPartialViewsFolder + "/_SliderQuestionOptions";
        private const string cStarsOptionsDetailsView = cPartialViewsFolder + "/_StarsQuestionDetails";
        private const string cSmileyOptionsDetailsView = cPartialViewsFolder + "/_SmileyQuestionDetails";
        private const string cSliderOptionsDetailsView = cPartialViewsFolder + "/_SliderQuestionDetails";
        //stars question properties
        const string cNumberOfStars = "NumberOfStars";
        //Smiley table
        private const string cNumberOfFaces = "NumberOfSmileyFaces";
        //Slider table
        private const string cStartValue = "StartValue";
        private const string cEndValue = "EndValue";
        private const string cStartValueCaption = "StartValueCaption";
        private const string cEndValueCaption = "EndValueCaption";

        public QuestionsController()
        {
            try
            {
            QuestionOperations.GetConnectionString();
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                RedirectToErrorPage("An error occured while loading the data");
            }
        }
        // GET: Questions
        public ActionResult Index()
        {
            try 
            { 
                var canGetQuesitons = QuestionOperations.GetQuestions();
                if (canGetQuesitons.IsSuccess && canGetQuesitons!=null)
                {
                    var model = QuestionOperations.mQuestionsList;
                    return View(model);
                }
                //handle case of failure to obtain questions
                return RedirectToErrorPage("An error occured while loading the data");
            }
            catch(Exception ex){
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
            }
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
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
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
                if (tQuestionToAdd != null) 
                { 
                    OperationResult tIsQuestionAdded = QuestionOperations.AddQuestion(tQuestionToAdd);
                    if (tIsQuestionAdded.IsSuccess)
                    {
                        //show pop up message
                        //on a successful question creation
                        return RedirectToAction(cQuestionsView);
                    }
                }
                //show error pop up
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                //fetch all question data
                Question tQuestionData = QuestionOperations.GetQuestionData(id);
                if (tQuestionData != null)
                {
                    return View(tQuestionData);
                }
                //handle the case of question not found
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Question pQuestionData, FormCollection pFormData)
        {
            try
            {

                Question tQuestionToAdd = CreateQuestionObject(pQuestionData, pFormData);

                OperationResult tIsQuestionAdded = QuestionOperations.UpdateQuestion(tQuestionToAdd);
                if (tIsQuestionAdded.IsSuccess)
                {
                    //show pop up message
                    //on a successful question creation
                    return RedirectToAction(cQuestionsView);
                }
                //show error pop up
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
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
                //maybe change this ?
                return RedirectToErrorPage("Question was not found");
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
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
                    //show popup message
                    //show error in deletion
                    return RedirectToAction(cQuestionsView);
                }
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
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
                //show popup message
                //show error in deletion
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
            }
        }

        #region class utility functions
        [HttpGet]
        public ActionResult GetQuestionTypeOptions(int pType)
        {
            try 
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
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
            }
        }

        [HttpGet]
        public ActionResult GetQuestionTypeDetailsEdit(int id)
        {
            try
            {
                Question tQuestionTypeData = null;
                OperationResult tCanGetQuestionTypeData = QuestionOperations.GetQuestionSpecificData(id, ref tQuestionTypeData);
                if (tCanGetQuestionTypeData.IsSuccess)
                {
                    //return the required partial view filled with data based on question type
                    return GetQuestionTypeDetailsPartialViewEdit(tQuestionTypeData);
                }
                //handle failure case
                //show pop up
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
            }
        }

        [HttpGet]
        private ActionResult GetQuestionTypeDetailsPartialViewEdit(Question pQuestionData)
        {
            try 
            { 
                switch (pQuestionData.Type)
                {
                    case eQuestionType.Stars:
                        StarsQuestion tStarsQuestionData = (StarsQuestion)pQuestionData;
                        return PartialView(cStarsOptionsView, new StarsQuestionOptions(tStarsQuestionData.NumberOfStars));
                    case eQuestionType.Smiley:
                        SmileyQuestion tSmileyQuestionData = (SmileyQuestion)pQuestionData;
                        return PartialView(cSmileyOptionsView, new SmileyQuestionOptions(tSmileyQuestionData.NumberOfSmileyFaces));
                    case eQuestionType.Slider:
                        SliderQuestion tSliderQuestionData = (SliderQuestion)pQuestionData;
                        return PartialView(cSliderOptionsView, new SliderQuestionOptions(
                            tSliderQuestionData.StartValue,
                            tSliderQuestionData.EndValue,
                            tSliderQuestionData.StartValueCaption,
                            tSliderQuestionData.EndValueCaption
                            ));
                }
                return null;
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
            }
        }

        [HttpGet]
        public ActionResult GetQuestionTypeDetails(int id)
        {
            try
            {
                Question tQuestionTypeData = null;
                OperationResult tCanGetQuestionTypeData = QuestionOperations.GetQuestionSpecificData(id, ref tQuestionTypeData);
                if (tCanGetQuestionTypeData.IsSuccess)
                {
                    //return the required partial view filled with data based on question type
                    return GetQuestionTypeDetailsPartialView(tQuestionTypeData);
                }
                //handle failure case
                //show pop up
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
            }
        }

        [HttpGet]
        private ActionResult GetQuestionTypeDetailsPartialView(Question pQuestionData)
        {
            try 
            { 
                switch (pQuestionData.Type)
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
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage();
            }
        }

        private Question CreateQuestionObject(Question pQuestionData, FormCollection pFormData)
        {
            try 
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
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }
        }

        private ActionResult RedirectToErrorPage(string pErrorMessage = "Error occured while Loading your page, please try again")
        {
            try 
            { 
            return RedirectToAction("ErrorPage", "Error", new { ErrorMessage = pErrorMessage });
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return View("Error)");
            }
        }
        #endregion
    }
}