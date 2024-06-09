using System;
using System.ComponentModel.DataAnnotations;
namespace SharedResources.Models
{
    public class Question
    {
        /// <summary>
        /// main model for the Question object used in this application
        /// </summary>
        public int Id { get; set; }

        [Required(ErrorMessage ="The question text can't be empty")]
        [MaxLength(350,ErrorMessage ="The question text can't be more than 350 characters")]
        public string Text { get; set; }

        [Required(ErrorMessage ="The question order is required")]
        [Range(1,120,ErrorMessage ="The question order must be between 1 and 120")]
        public int Order { get; set; }

        [Required(ErrorMessage ="The question type is required")]
        public eQuestionType Type { get; set; }

        public Question()
        {

        }
        public Question(int pId, string pText, int pOrder, eQuestionType pType)
        {
            Id = pId;
            Text = pText;
            Order = pOrder;
            Type = pType;
        }
        public Question(string pText, int pOrder, eQuestionType pType)
        {
            Text = pText;
            Order = pOrder;
            Type = pType;
        }
    }
}
