﻿
using SharedResources;

namespace SurveyConfiguratorWeb.ConstantsAndMethods
{
    public class SharedConstants
    {
        //constants
        #region controllers, actions and views related constants
        //Error controller
        public const string cErrorController = "Error";
        public const string cErrorPageAction = "ErrorPage";

        //Options controller
        public const string cOptionsController = "Options";
        public const string cOptionsIndexAction = "Index";
        public const string cLanguageAction = "Language";
        public const string cConnectionSettingsAction = "ConnectionSettings";
        public const string cLagnaugeAppSettingKey = "Language";

        //Questions controller
        public const string cQuestionsController = "Questions";
        public const string cQuestionsIndexAction = "Index";
        public const string cQuestionCreateAction = "Create";
        public const string cQuestionEditAction = "Edit";
        public const string cQuestionDeleteAction = "Delete";
        public const string cQuestionDetailsAction = "Details";
        public const string cGetQuestionTypeOptionsFunction = "GetQuestionTypeOptions";
        public const string cGetQuestionTypeDetailsEditFunction = "GetQuestionTypeDetailsEdit";
        public const string cGetQuestionTypeDetailsFunction = "GetQuestionTypeDetails";
        public const string cGetQuestionsListPartialViewFunction = "GetQuestionsListPartialView";
        public const string cGetChecksumValueFunction = "GetChecksumValue";

        public const string cPartialViewsFolder = "PartialViews";
        public const string cQuestionsListView = cPartialViewsFolder + "/_QuestionsList";
        public const string cQuestionGeneralFieldsView = cPartialViewsFolder + "/_QuestionGeneralFields";

        #endregion


        #region errors and messages related constants
        // messages keys
        public const string cConnectionResultMessageKey = "ConnectionResult";
        public const string cMessageKey = "Message";

        //messages
        public const string cConnectionSuccessfulMessage = "Database connected successfully!";
        public const string cConnectionFailedMessage = "Database refused to connect";
        public static string cDefaultErrorMessage = GlobalStrings.PageLoadingError;
        #endregion

        #region html-related constants
        //ids
        public const string cLanguageDropDownId = "LanguageDropDown";
        public const string cQuestionTypeDropDownId = "QuestionTypeDropDown";
        public const string cQuestionTypeOptionsId = "QuestionTypeOptions";
        public const string cQuestionOperationFormId = "QuestionOperationForm";
        public const string cAlertDivId = "alert";

        #endregion
    }
}