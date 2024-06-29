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
        private string cDefaultErrorMessage = GlobalStrings.PageLoadingError;
        private const string cMessageKey = "Message";
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
                //test connection
                OperationResult tCanConnectToDb = QuestionOperations.TestDBConnection();
                if (!tCanConnectToDb.IsSuccess)
                {
                    //redirect to appropriate error page
                }

            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                //the redirect here doesn't work
                RedirectToErrorPage(cDefaultErrorMessage);
            }
        }
        // GET: Questions
        public ActionResult Index()
        {
            try 
            { 
                //get all questions data
                var canGetQuesitons = QuestionOperations.GetQuestions();
                if (canGetQuesitons.IsSuccess && canGetQuesitons!=null)
                {
                    return View(GetQuestionsData());
                }
                //handle case of failure to obtain questions
                return RedirectToAction(GlobalStrings.DataFetchingError);
            }
            catch(Exception ex){
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(cDefaultErrorMessage);
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
                return RedirectToErrorPage(cDefaultErrorMessage);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuestionViewModel pQuestionModelData, FormCollection pFormData)
        {
            try
            {
                //based on the type of question create a new object and
                //fill its respective fields
                Question tQuestionToAdd = CreateQuestionObject(pQuestionModelData, pFormData);
                if (tQuestionToAdd != null)
                {
                    OperationResult tIsQuestionAdded = QuestionOperations.AddQuestion(tQuestionToAdd);
                    if (tIsQuestionAdded.IsSuccess)
                    {
                        //show pop up message
                        //on a successful question creation
                        TempData[cMessageKey] = GlobalStrings.OperationSuccessful;
                        return RedirectToAction(cQuestionsView);
                    }
                }
                TempData[cMessageKey] = GlobalStrings.OperationError;
                //show error pop up
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(cDefaultErrorMessage);
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
                    QuestionViewModel tQuestionModelData = new QuestionViewModel(
                        tQuestionData.Id,
                        tQuestionData.Text,
                        tQuestionData.Order,
                        tQuestionData.Type
                        );
                    return View(tQuestionModelData);
                }
                TempData[cMessageKey] = GlobalStrings.QuestionDataFetchingError;
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(cDefaultErrorMessage);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuestionViewModel pQuestionModelUpdatedData, FormCollection pFormData)
        {
            try
            {

                Question tQuestionToAdd = CreateQuestionObject(pQuestionModelUpdatedData, pFormData);

                OperationResult tIsQuestionAdded = QuestionOperations.UpdateQuestion(tQuestionToAdd);
                if (tIsQuestionAdded.IsSuccess)
                {
                    TempData[cMessageKey] = GlobalStrings.OperationSuccessful;
                    return RedirectToAction(cQuestionsView);
                }
                TempData[cMessageKey] = GlobalStrings.OperationError;
                //show error pop up
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(cDefaultErrorMessage);
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
                    QuestionViewModel tQuestionModelData = new QuestionViewModel(
                        tQuestionData.Id,
                        tQuestionData.Text,
                        tQuestionData.Order,
                        tQuestionData.Type
                        );
                    return View(tQuestionModelData);
                }
                TempData["Message"] = GlobalStrings.QuestionDataFetchingError;
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(cDefaultErrorMessage);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(QuestionViewModel pQuestionData)
        {
            try
            {
                List<int> tQuestionsIds = new List<int>();
                tQuestionsIds.Add(pQuestionData.Id);
                OperationResult tAreQuestionsDeleted = QuestionOperations.DeleteQuestion(tQuestionsIds);
                if (tAreQuestionsDeleted.IsSuccess)
                {
                    TempData[cMessageKey] = GlobalStrings.OperationSuccessful;
                    return RedirectToAction(cQuestionsView);
                }
                else
                {
                    TempData[cMessageKey] = GlobalStrings.OperationError;
                    return RedirectToAction(cQuestionsView);
                }
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(cDefaultErrorMessage);
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
                    QuestionViewModel tQuestionModelData = new QuestionViewModel(
                        tQuestionData.Id,
                        tQuestionData.Text,
                        tQuestionData.Order,
                        tQuestionData.Type
                        );
                    return View(tQuestionModelData);
                }
                TempData[cMessageKey] = GlobalStrings.OperationError;
                return RedirectToAction(cQuestionsView);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(cDefaultErrorMessage);
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
                return RedirectToErrorPage(cDefaultErrorMessage);
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
                return RedirectToErrorPage(cDefaultErrorMessage);
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
                return RedirectToErrorPage(cDefaultErrorMessage);
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
                return RedirectToErrorPage(cDefaultErrorMessage);
            }
        }

        [HttpGet]
        public ActionResult GetQuestionsListPartialView()
        {
            try
            {
                var canGetQuesitons = QuestionOperations.GetQuestions();
                if (canGetQuesitons != null && canGetQuesitons.IsSuccess) { 
                    IEnumerable<QuestionViewModel> tQuestionsListViewModel = GetQuestionsData();
                    return PartialView("PartialViews/_QuestionsList", tQuestionsListViewModel);
                }
                //handle faliure case 
                return RedirectToErrorPage(GlobalStrings.DataBaseConnectionError);
            }
            catch( Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(cDefaultErrorMessage);
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
                return RedirectToErrorPage(cDefaultErrorMessage);
            }
        }

        public IEnumerable<QuestionViewModel> GetQuestionsData()
        {
            List<Question> tQuestionsList = QuestionOperations.mQuestionsList;

            List<QuestionViewModel> tModelQuestionsList = new List<QuestionViewModel>();
            foreach (Question tQuestion in tQuestionsList)
            {
                tModelQuestionsList.Add(new QuestionViewModel(
                    tQuestion.Id,
                    tQuestion.Text,
                    tQuestion.Order,
                    tQuestion.Type
                    ));
            }
            return tModelQuestionsList;
        }

        public long GetChecksumValue()
        {
            try
            {
                long tChecksumValue = 0;
                OperationResult tCheckSumResult = QuestionOperations.GetDataBaseChecksum(ref tChecksumValue);
                return tChecksumValue;
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                return 0;
            }
        }

        private Question CreateQuestionObject(QuestionViewModel pQuestionModelData, FormCollection pFormData)
        {
            //encapsulate the questionViewModel in a Question object

            Question tQuestionData= new Question(
                pQuestionModelData.Id,
                pQuestionModelData.Text,
                pQuestionModelData.Order,
                pQuestionModelData.Type
                );

            try 
            { 
                Question tCreatedQuestion = null;
                switch (tQuestionData.Type)
                {
                    case eQuestionType.Stars:
                        tCreatedQuestion = new StarsQuestion(tQuestionData, Convert.ToInt32(pFormData[cNumberOfStars]));
                        break;
                    case eQuestionType.Smiley:
                        tCreatedQuestion = new SmileyQuestion(tQuestionData, Convert.ToInt32(pFormData[cNumberOfFaces]));
                        break;
                    case eQuestionType.Slider:
                        tCreatedQuestion = new SliderQuestion
                            (
                            tQuestionData,
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

        private ActionResult RedirectToErrorPage(string pErrorMessage)
        {
            try 
            { 
            return RedirectToAction("ErrorPage", "Error", new { ErrorMessage = pErrorMessage });
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return View("Error");
            }
        }
        #endregion
    }
}