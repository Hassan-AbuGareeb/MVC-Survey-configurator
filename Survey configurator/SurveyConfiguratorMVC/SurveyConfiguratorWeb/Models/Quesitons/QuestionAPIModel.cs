using SharedResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SurveyConfiguratorWeb.Models.Quesitons
{
    public class QuestionAPIModel
    {


        //general question attributes
        public int Id { get; set; }
        public string Text { get; set; }
        public int Order { get; set; }
        public eQuestionType Type { get; set; }

        //question type specific attributes
        //Stars question
        public int NumberOfStars { get; set; }

        //Smiley question
        public int NumberOfSmileyFaces { get; set; }

        //slider question
        public int StartValue { get; set; }
        public int EndValue { get; set; }
        public string StartValueCaption { get; set; }
        public string EndValueCaption { get; set; }


        public QuestionAPIModel()
        {

        }
        //public QuestionAPIModel(int pId, string pText, int pOrder, eQuestionType pType)
        //{
        //    Id = pId;
        //    Text = pText;
        //    Order = pOrder;
        //    Type = pType;
        //}
        //public QuestionAPIModel(string pText, int pOrder, eQuestionType pType)
        //{
        //    Text = pText;
        //    Order = pOrder;
        //    Type = pType;
        //}
    }
}