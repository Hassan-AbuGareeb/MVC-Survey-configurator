using QuestionServices;
using SharedResources;
using SharedResources.Models;
using SurveyConfiguratorWeb.ConstantsAndMethods;
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
        /// <summary>
        /// responsible for the most functionality of the app.
        /// showing questions objects in list, creating, editing and deleting
        /// quesitions
        /// </summary>

        //constants
        private const string cStarsOptionsView = SharedConstants.cPartialViewsFolder + "/_StarsQuestionOptions";
        private const string cSmileyOptionsView = SharedConstants.cPartialViewsFolder + "/_SmileyQuestionOptions";
        private const string cSliderOptionsView = SharedConstants.cPartialViewsFolder + "/_SliderQuestionOptions";
        private const string cStarsOptionsDetailsView = SharedConstants.cPartialViewsFolder + "/_StarsQuestionDetails";
        private const string cSmileyOptionsDetailsView = SharedConstants.cPartialViewsFolder + "/_SmileyQuestionDetails";
        private const string cSliderOptionsDetailsView = SharedConstants.cPartialViewsFolder + "/_SliderQuestionDetails";

        //stars question properties
        private const string cNumberOfStars = "NumberOfStars";
        //Smiley table
        private const string cNumberOfFaces = "NumberOfSmileyFaces";
        //Slider table
        private const string cStartValue = "StartValue";
        private const string cEndValue = "EndValue";
        private const string cStartValueCaption = "StartValueCaption";
        private const string cEndValueCaption = "EndValueCaption";

        /// <summary>
        /// constructor for the controller, checks for the database connectivity before 
        /// doing any operation
        /// </summary>
        public QuestionsController()
        {
            //try
            //{
            //    //test connection
            //    OperationResult tCanConnectToDb = QuestionOperations.TestDBConnection();
            //    if (!tCanConnectToDb.IsSuccess)
            //    {
            //        //redirect to appropriate error page
            //    }
            //}
            //catch(Exception ex)
            //{
            //    UtilityMethods.LogError(ex);
            //    //the redirect here doesn't work
            //    RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            //}
        }

        /// <summary>
        /// shows the Questions list view and redirectes to error page in case of failure
        /// </summary>
        /// <returns>view containing questions list</returns>
        [HttpGet]
        public ActionResult Index()
        {
            try 
            { 
                //get all questions data
                OperationResult tCanGetQuesitons = QuestionOperations.GetQuestions();
                if (tCanGetQuesitons.IsSuccess && tCanGetQuesitons != null)
                {
                    return View(GetQuestionsData());
                }
                //handle case of failure to obtain questions
                return RedirectToAction(GlobalStrings.DataFetchingError);
            }
            catch(Exception ex){
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        /// <summary>
        /// shows the create question view
        /// </summary>
        /// <returns>create question view</returns>

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
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        /// <summary>
        /// create a question object from the question view model
        /// and add it to the database
        /// </summary>
        /// <param name="pQuestionModelData">question general data</param>
        /// <param name="pFormData">contains the question-type data</param>
        /// <returns></returns>
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
                        return RedirectToAction(SharedConstants.cQuestionsIndexAction);
                    }
                }
                TempData[SharedConstants.cMessageKey] = GlobalStrings.OperationError;
                //show error pop up, failed in adding the question
                return RedirectToAction(SharedConstants.cQuestionsIndexAction);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        /// <summary>
        /// shows the edit question view with the question
        /// fields filled with the questions data
        /// </summary>
        /// <param name="id">question Id</param>
        /// <returns>edit question view</returns>
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
                TempData[SharedConstants.cMessageKey] = GlobalStrings.QuestionDataFetchingError;
                return RedirectToAction(SharedConstants.cQuestionsIndexAction);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        /// <summary>
        /// edits the question data received from the 
        /// edit question view
        /// </summary>
        /// <param name="pQuestionModelUpdatedData">updated data</param>
        /// <param name="pFormData">contains question-type data</param>
        /// <returns>a view to get redirected to</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuestionViewModel pQuestionModelUpdatedData, FormCollection pFormData)
        {
            try
            {
                Question tQuestionToAdd = CreateQuestionObject(pQuestionModelUpdatedData, pFormData);
                //update question data
                OperationResult tIsQuestionAdded = QuestionOperations.UpdateQuestion(tQuestionToAdd);
                if (tIsQuestionAdded.IsSuccess)
                {
                    return RedirectToAction(SharedConstants.cQuestionsIndexAction);
                }
                TempData[SharedConstants.cMessageKey] = GlobalStrings.OperationError;
                //show error pop up
                return RedirectToAction(SharedConstants.cQuestionsIndexAction);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        /// <summary>
        /// shows a view with the question general data
        /// to delete that question
        /// </summary>
        /// <param name="id">question id</param>
        /// <returns>delete question view</returns>
        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                //get question data
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
                TempData[SharedConstants.cMessageKey] = GlobalStrings.QuestionDataFetchingError;
                return RedirectToAction(SharedConstants.cDefaultErrorMessage);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cQuestionsIndexAction);
            }
        }

        /// <summary>
        /// deletes question using its id, the id is added 
        /// to a list, this implementation supports deleting
        /// multiple questions at once
        /// </summary>
        /// <param name="pQuestionData">question data</param>
        /// <returns> a view to be redirected to</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(QuestionViewModel pQuestionData)
        {
            try
            {
                List<int> tQuestionsIds = new List<int>
                {
                    pQuestionData.Id
                };
                //delete questions
                OperationResult tAreQuestionsDeleted = QuestionOperations.DeleteQuestion(tQuestionsIds);
                if (tAreQuestionsDeleted.IsSuccess)
                {
                    return RedirectToAction(SharedConstants.cQuestionsIndexAction);
                }
                //show error pop up
                TempData[SharedConstants.cMessageKey] = GlobalStrings.OperationError;
                return RedirectToAction(SharedConstants.cQuestionsIndexAction);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        /// <summary>
        /// show the data of the question
        /// including the general data and the
        /// question-type data
        /// </summary>
        /// <param name="id">question id</param>
        /// <returns>view containing the question details</returns>
        [HttpGet]
        public ActionResult Details(int id)
        {
            try
            {
                //get question data
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
                TempData[SharedConstants.cMessageKey] = GlobalStrings.OperationError;
                return RedirectToAction(SharedConstants.cQuestionsIndexAction);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
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
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
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
                return RedirectToAction(SharedConstants.cQuestionsIndexAction);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
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
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        [HttpGet]
        public ActionResult GetQuestionsListPartialView()
        {
            try
            {
                //get all questions data
                OperationResult tCanGetQuesitons = QuestionOperations.GetQuestions();
                if (tCanGetQuesitons != null && tCanGetQuesitons.IsSuccess) { 
                    //put the questions data in a list of question view model objects
                    IEnumerable<QuestionViewModel> tQuestionsListViewModel = GetQuestionsData();
                    return PartialView(SharedConstants.cQuestionsListView, tQuestionsListViewModel);
                }
                //handle faliure case 
                return RedirectToErrorPage(GlobalStrings.DataBaseConnectionError);
            }
            catch( Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        [HttpGet]
        public ActionResult GetQuestionTypeDetails(int id)
        {
            try
            {
                Question tQuestionTypeData = null;
                //get full question data
                OperationResult tCanGetQuestionTypeData = QuestionOperations.GetQuestionSpecificData(id, ref tQuestionTypeData);
                if (tCanGetQuestionTypeData.IsSuccess)
                {
                    //return the required partial view filled with data based on question type
                    return GetQuestionTypeDetailsPartialView(tQuestionTypeData);
                }
                //handle failure case
                //show pop up
                return RedirectToAction(SharedConstants.cQuestionsIndexAction);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

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
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        /// <summary>
        /// creates a list of QuestionViewModel objects to use as model in the 
        /// Index action of this controller
        /// </summary>
        /// <returns>an IEnumerable collection containing QuestionViewModel objects</returns>
        public IEnumerable<QuestionViewModel> GetQuestionsData()
        {
            try { 
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
            catch( Exception ex )
            {
                UtilityMethods.LogError(ex);
                //handle the exception
                //return this or null
                return new List<QuestionViewModel>();
            }
        }

        /// <summary>
        /// returns the database checksum value
        /// </summary>
        /// <returns>long value representing the checksum value</returns>
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

        /// <summary>
        /// creates a question object from the QuestionViewModel object and theh form data
        /// </summary>
        /// <param name="pQuestionModelData">question general data</param>
        /// <param name="pFormData">contains question-type data</param>
        /// <returns>Question object</returns>
        private Question CreateQuestionObject(QuestionViewModel pQuestionModelData, FormCollection pFormData)
        {
            try
            {
                //encapsulate the questionViewModel in a Question object
                Question tQuestionData= new Question(
                pQuestionModelData.Id,
                pQuestionModelData.Text,
                pQuestionModelData.Order,
                pQuestionModelData.Type
                );

                Question tCreatedQuestion = null;
                //add the question-type data to the question object;
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

        /// <summary>
        /// redirects to the error view
        /// </summary>
        /// <param name="pErrorMessage"> message to show on the error page </param>
        /// <returns></returns>
        private ActionResult RedirectToErrorPage(string pErrorMessage)
        {
            try 
            { 
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController, new { ErrorMessage = pErrorMessage });
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController, new { ErrorMessage = pErrorMessage });
            }
        }
        #endregion
    }
}