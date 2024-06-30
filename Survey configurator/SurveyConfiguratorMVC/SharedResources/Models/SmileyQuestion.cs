using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Models
{
    public class SmileyQuestion : Question
    {
        public string Text { get; set; }
        public int Order { get; set; }
        public int NumberOfSmileyFaces { get; set; }

        public SmileyQuestion(int pId, string pText, int pOrder, int pNumberOfSmileyFaces = SharedData.cMaxNumberOfSmileyFaces) : base(pId, pText, pOrder, eQuestionType.Smiley)
        {
            NumberOfSmileyFaces = pNumberOfSmileyFaces;
        }

        public SmileyQuestion(Question pQuestionData, int pNumberOfSmileyFaces = SharedData.cMaxNumberOfSmileyFaces) : base(pQuestionData.Id, pQuestionData.Text, pQuestionData.Order, eQuestionType.Smiley)
        {
            NumberOfSmileyFaces = pNumberOfSmileyFaces;
        }
    }
}
