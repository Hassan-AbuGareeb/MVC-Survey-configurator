using SharedResources;
using System;
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

        [Required(ErrorMessage = "The question text can't be empty")]
        [MaxLength(SharedData.cQuestionTextLength, ErrorMessage = "The question text can't be more than 350 characters")]
        public string Text { get; set; }

        [Required(ErrorMessage = "The question order is required")]
        [Range(1, Int32.MaxValue, ErrorMessage = "The question order must be greater than 1")]
        public int Order { get; set; }

        [Required(ErrorMessage = "The question type is required")]
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