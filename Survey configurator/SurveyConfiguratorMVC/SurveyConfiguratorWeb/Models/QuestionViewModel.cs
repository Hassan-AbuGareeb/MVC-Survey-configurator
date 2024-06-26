using SharedResources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SurveyConfiguratorWeb.Models
{
    public class QuestionViewModel
    {
        /// <summary>
        /// view model for the question object partial view
        /// which contains only the general question info
        /// with the appropriate data annotations to help 
        /// with the validation.
        /// the errors messages are fetched from the resource file to 
        /// enable the localization 
        /// </summary>


        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalStrings),
            ErrorMessageResourceName = "QuestionTextRequiredError")]
        [MaxLength(SharedData.cQuestionTextLength,
            ErrorMessageResourceType =typeof(GlobalStrings), ErrorMessageResourceName = "QuestionTextLengthError")]
        public string Text { get; set; }

        [Required( ErrorMessageResourceType = typeof(GlobalStrings),
            ErrorMessageResourceName = "QuestionOrderRequiredError")]
        [Range(1, Int32.MaxValue, 
            ErrorMessageResourceType =typeof(GlobalStrings),
            ErrorMessageResourceName = "QuestionOrderValueError")]
        public int Order { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalStrings), 
            ErrorMessageResourceName = "QuestionTypeError")]
        public eQuestionType Type { get; set; }

        public QuestionViewModel()
        {

        }

        public QuestionViewModel(int pId, string pText, int pOrder, eQuestionType pType)
        {
            Id = pId;
            Text = pText;
            Order = pOrder;
            Type = pType;
        }

        public QuestionViewModel(string pText, int pOrder, eQuestionType pType)
        {
            Text = pText;
            Order = pOrder;
            Type = pType;
        }
    }
}