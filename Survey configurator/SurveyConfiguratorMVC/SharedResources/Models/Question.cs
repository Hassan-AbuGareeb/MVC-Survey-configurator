using System;

namespace SharedResources.Models
{
    public class Question
    {
        /// <summary>
        /// main model for the Question object used in this application
        /// </summary>

        public int Id { get; set; }
        public string Text { get; set; }
        public int Order { get; set; }
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
