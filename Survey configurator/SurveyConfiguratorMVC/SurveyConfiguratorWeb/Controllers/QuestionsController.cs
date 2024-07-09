using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using QuestionServices;
using SharedResources;
using SharedResources.Models;
using SurveyConfiguratorWeb.Attributes;
using SurveyConfiguratorWeb.ConstantsAndMethods;
using SurveyConfiguratorWeb.Filters;
using SurveyConfiguratorWeb.Models;
using SurveyConfiguratorWeb.Models.Quesitons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Web.Services.Description;

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
        /// shows the questions list view
        /// </summary>
        /// <returns>a view containing the list of questions</returns>
        public ActionResult Index()
        {
            try 
            { 
                //get all questions data
                if(QuestionOperations.mIsDataBaseConnected) { 
                    OperationResult canGetQuesitons = QuestionOperations.GetQuestions();
                    if (canGetQuesitons.IsSuccess && canGetQuesitons!=null)
                    {
                        IEnumerable<QuestionViewModel> tQuestionsData = GetQuestionsData();
                        if (tQuestionsData != null) 
                        { 
                            return View(GetQuestionsData());
                        }
                        else
                        {
                            return RedirectToErrorPage(GlobalStrings.DataLoadingError);
                        }
                    }
                    //handle case of failure to obtain questions
                    return RedirectToErrorPage(GlobalStrings.DataLoadingError);
                }
                return RedirectToErrorPage(GlobalStrings.DataBaseConnectionError);
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
        [Authenticated]
        public ActionResult Create()
        {
            try
            {
                if (QuestionOperations.mIsDataBaseConnected)
                {
                    return View();
                }
                return RedirectToErrorPage(GlobalStrings.DataBaseConnectionError);
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
        [Authenticated]
        public ActionResult Create(QuestionViewModel pQuestionModelData, FormCollection pFormData)
        {
            try
            {
                //based on the type of question create a new object and
                //fill its respective fields
                if (QuestionOperations.mIsDataBaseConnected)
                {
                    Question tQuestionToAdd = CreateQuestionObject(pQuestionModelData, pFormData);
                    if (tQuestionToAdd != null)
                    {
                        //question object created
                        OperationResult tIsQuestionAdded = QuestionOperations.AddQuestion(tQuestionToAdd);
                        if (tIsQuestionAdded.IsSuccess)
                        {
                            //redirect to questions list on successful operation
                            return RedirectToAction(SharedConstants.cQuestionsIndexAction);
                        }
                    }
                    //show error pop up and redirect to questionslist page
                    TempData[SharedConstants.cMessageKey] = GlobalStrings.OperationError;
                    return RedirectToAction(SharedConstants.cQuestionsIndexAction);
                }
                return RedirectToErrorPage(GlobalStrings.DataBaseConnectionError);
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
        [Authenticated]
        public ActionResult Edit(int id)
        {
            try
            {
                if (QuestionOperations.mIsDataBaseConnected)
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
                return RedirectToErrorPage(GlobalStrings.DataBaseConnectionError);
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
        [Authenticated]
        public ActionResult Edit(QuestionViewModel pQuestionModelUpdatedData, FormCollection pFormData)
        {
            try
            {
                if (QuestionOperations.mIsDataBaseConnected)
                {
                    //extract question data from the questionViewmodel object and the form data
                    Question tQuestionToAdd = CreateQuestionObject(pQuestionModelUpdatedData, pFormData);

                    OperationResult tIsQuestionAdded = QuestionOperations.UpdateQuestion(tQuestionToAdd);
                    if (tIsQuestionAdded.IsSuccess)
                    {
                        return RedirectToAction(SharedConstants.cQuestionsIndexAction);
                    }
                    //show error pop up
                    TempData[SharedConstants.cMessageKey] = GlobalStrings.OperationError;
                    return RedirectToAction(SharedConstants.cQuestionsIndexAction);
                }
                return RedirectToErrorPage(GlobalStrings.DataBaseConnectionError);
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
        [Authenticated]
        public ActionResult Delete(int id)
        {
            try
            {
                if (QuestionOperations.mIsDataBaseConnected)
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
                    return RedirectToAction(SharedConstants.cQuestionsIndexAction);
                }
                return RedirectToErrorPage(GlobalStrings.DataBaseConnectionError);
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
        [Authenticated]
        public ActionResult Delete(QuestionViewModel pQuestionData)
        {
            try
            {
                if (QuestionOperations.mIsDataBaseConnected)
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
                    else
                    {
                        //show error pop up
                        TempData[SharedConstants.cMessageKey] = GlobalStrings.OperationError;
                        return RedirectToAction(SharedConstants.cQuestionsIndexAction);
                    }
                }
                return RedirectToErrorPage(GlobalStrings.DataBaseConnectionError);
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
                if (QuestionOperations.mIsDataBaseConnected)
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
                return RedirectToErrorPage(GlobalStrings.DataBaseConnectionError);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        #region class utility functions

        /// <summary>
        /// returns a partial view containing question-type options
        /// based on the requested question type for the "create" view
        /// </summary>
        /// <param name="pType"> question type </param>
        /// <returns>partial view with the question-type options</returns>
        [HttpGet]
        public ActionResult GetQuestionTypeOptions(int pType)
        {
            try 
            {
                eQuestionType tQuestionType = (eQuestionType)pType;
                string tOptionViewViewName = GetViewName(tQuestionType);
                //return view based on function result
                return PartialView(tOptionViewViewName);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        /// <summary>
        /// this function returns a partial view for the edit view, containing
        /// question-type options, filled with the question data.
        /// </summary>
        /// <param name="id">question id</param>
        /// <returns>partial view with question-type options</returns>
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
                //show pop up
                return RedirectToAction(SharedConstants.cQuestionsIndexAction);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToErrorPage(SharedConstants.cDefaultErrorMessage);
            }
        }

        /// <summary>
        /// returns a partial view with the requested question-type
        /// fields filled with the question data, for viewing only.
        /// </summary>
        /// <param name="pQuestionData"></param>
        /// <returns> partial view with question-type fields</returns>
        private ActionResult GetQuestionTypeDetailsPartialViewEdit(Question pQuestionData)
        {
            try 
            { 
                switch (pQuestionData.Type)
                {
                    case eQuestionType.Stars:
                        StarsQuestion tStarsQuestionData = (StarsQuestion)pQuestionData;
                        return PartialView(GetViewName(eQuestionType.Stars), new StarsQuestionOptions(tStarsQuestionData.NumberOfStars));
                    case eQuestionType.Smiley:
                        SmileyQuestion tSmileyQuestionData = (SmileyQuestion)pQuestionData;
                        return PartialView(GetViewName(eQuestionType.Smiley), new SmileyQuestionOptions(tSmileyQuestionData.NumberOfSmileyFaces));
                    case eQuestionType.Slider:
                        SliderQuestion tSliderQuestionData = (SliderQuestion)pQuestionData;
                        return PartialView(GetViewName(eQuestionType.Slider), new SliderQuestionOptions(
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

        /// <summary>
        /// this function returns a partial view based on the requested
        /// question type
        /// </summary>
        /// <param name="tQuestionType">type of the question</param>
        /// <returns>a string representing the requried partial view name</returns>
        private string GetViewName(eQuestionType tQuestionType)
        {
            try
            {
                string tOptionViewName = "";
                switch (tQuestionType)
                {
                    case eQuestionType.Stars:
                        tOptionViewName = cStarsOptionsView;
                        break;
                    case eQuestionType.Smiley:
                        tOptionViewName = cSmileyOptionsView;
                        break;
                    case eQuestionType.Slider:
                        tOptionViewName = cSliderOptionsView;
                        break;
                }
                return tOptionViewName;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return string.Empty;
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
        /// fetch the questions from the database encapsulates them in
        /// QuestionViewModel and return a partial view with the qeustions
        /// data
        /// </summary>
        /// <returns>partial view of the questions datat</returns>
        [HttpGet]
        public ActionResult GetQuestionsListPartialView()
        {
            try
            {
                if (QuestionOperations.mIsDataBaseConnected) { 
                    //get all questions data
                    OperationResult tCanGetQuesitons = QuestionOperations.GetQuestions();
                    if (tCanGetQuesitons != null && tCanGetQuesitons.IsSuccess)
                    {
                        //put the questions data in a list of question view model objects
                        IEnumerable<QuestionViewModel> tQuestionsListViewModel = GetQuestionsData();
                        return PartialView(SharedConstants.cQuestionsListView, tQuestionsListViewModel);
                    }
                    //handle faliure case 
                    return RedirectToErrorPage(GlobalStrings.DataLoadingError);
                }
                return RedirectToErrorPage(GlobalStrings.DataBaseConnectionError);
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
                return null;
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
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController);
            }
        }
        #endregion

        #region api functions

        /// <summary>
        /// return json object containing a list of question
        /// objects data
        /// </summary>
        /// <returns>json object containing all questions data</returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                if (QuestionOperations.mIsDataBaseConnected)
                {
                    OperationResult canGetQuesitons = QuestionOperations.GetQuestions();
                    if (canGetQuesitons.IsSuccess && canGetQuesitons != null)
                    {
                        //success
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return Json(new { Questions = QuestionOperations.mQuestionsList }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //error while loading the data
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return Json(new {message = GlobalStrings.DataLoadingError }, JsonRequestBehavior.AllowGet);
                    }
                }
                //db disconnected
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { message = GlobalStrings.DataBaseConnectionError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { message = GlobalStrings.UnknownError }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// creates a question based on the received data
        /// </summary>
        /// <param name="QuestionData"> question data</param>
        /// <returns>http response with a message indicating success of the operation</returns>

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Add(QuestionAPIModel QuestionData)
        {
            try
            {
                if (QuestionOperations.mIsDataBaseConnected)
                {
                    Question tQuestionObject = CreateQuesitonObject(QuestionData);
                    if (tQuestionObject != null)
                    {
                        //question object successfully created
                        //add to database
                        OperationResult tQuestionAddedResult = QuestionOperations.AddQuestion(tQuestionObject);
                        if (tQuestionAddedResult.IsSuccess)
                        {
                            return Json(new { Message = GlobalStrings.OperationSuccessful });
                        }
                        //most likely a validation errer
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(new { Message = tQuestionAddedResult.mErrorMessage });
                    }
                    //error in creating question object from recived input
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { Message = GlobalStrings.NullValueError });
                }
                //db disconnected
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { message = GlobalStrings.DataBaseConnectionError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { message = GlobalStrings.UnknownError });
            }
        }

        /// <summary>
        /// updates question based on the received data
        /// </summary>
        /// <param name="UpdatedQuestionData">new question data</param>
        /// <returns>http response with a message indicating success of the operation</returns>
        [AllowAnonymous]
        [HttpPut]
        public ActionResult Update(QuestionAPIModel UpdatedQuestionData)
        {
            try
            {
                if (QuestionOperations.mIsDataBaseConnected)
                {
                    Question tQuestionObject = CreateQuesitonObject(UpdatedQuestionData);
                    if (tQuestionObject != null)
                    {
                        //question object successfully created
                        //update question data to database
                        OperationResult tQuestionUpdatedResult = QuestionOperations.UpdateQuestion(tQuestionObject);
                        if (tQuestionUpdatedResult.IsSuccess)
                        {
                            return Json(new { Message = GlobalStrings.OperationSuccessful });
                        }
                        //most likely a validation errer
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(new { Message = tQuestionUpdatedResult.mErrorMessage });
                    }
                    //error in creating question object from data
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { Message = GlobalStrings.NullValueError });
                }
                //db disconnected
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { message = GlobalStrings.DataBaseConnectionError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { message = GlobalStrings.UnknownError });
            }
        }

        /// <summary>
        /// deletes a question object based on the received id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>http response with a message indicating success of the operation</returns>
        [AllowAnonymous]
        [HttpDelete]
        public ActionResult Remove(int id)
        {
            try
            {
                if (QuestionOperations.mIsDataBaseConnected)
                {
                    List<int> tQuestionsIds = new List<int>
                    {
                        id
                    };
                    //delete questions
                    OperationResult tAreQuestionsDeleted = QuestionOperations.DeleteQuestion(tQuestionsIds);
                    if (tAreQuestionsDeleted.IsSuccess)
                    {
                        //successful deletion
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return Json(new { Message = GlobalStrings.OperationSuccessful }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //most likely an invalid question id
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return Json(new { message = GlobalStrings.DeleteQuestionError });
                    }
                }
                //db disconnected
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { message = GlobalStrings.DataBaseConnectionError });
            }
            catch(Exception ex) 
            {
                UtilityMethods.LogError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { message = GlobalStrings.UnknownError });
            }
        }


        //api utility functions

        /// <summary>
        /// construct a question object from the question
        /// api model object received.
        /// </summary>
        /// <param name="pQuestionData">full question data including all fields</param>
        /// <returns>a quesiton object</returns>
        private static Question CreateQuesitonObject(QuestionAPIModel pQuestionData)
        {
            try
            {
                switch (pQuestionData.Type)
                {
                    case eQuestionType.Stars:
                        return new StarsQuestion(pQuestionData.Id, pQuestionData.Text, pQuestionData.Order, pQuestionData.NumberOfStars);

                    case eQuestionType.Smiley:
                        return new SmileyQuestion(pQuestionData.Id, pQuestionData.Text, pQuestionData.Order, pQuestionData.NumberOfSmileyFaces);

                    case eQuestionType.Slider:
                        return new SliderQuestion(pQuestionData.Id, pQuestionData.Text, pQuestionData.Order,
                            pQuestionData.StartValue,
                            pQuestionData.EndValue,
                            pQuestionData.StartValueCaption,
                            pQuestionData.EndValueCaption);
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }
        }

        #endregion
    }
}