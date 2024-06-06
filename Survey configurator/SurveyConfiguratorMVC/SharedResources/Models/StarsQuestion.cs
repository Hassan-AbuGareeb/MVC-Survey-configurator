using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Models
{
    public class StarsQuestion : Question
    {
        public string Text { get; set; }
        public int Order { get; set; }
        public int NumberOfStars { get; set; }

        public StarsQuestion(int pId, string pText, int pOrder, int pNumberOfStars = 5) : base(pId, pText, pOrder, eQuestionType.Stars)
        {
            NumberOfStars = pNumberOfStars;
        }

        public StarsQuestion(Question pQuestionData, int pNumberOfstars) : base(pQuestionData.Id, pQuestionData.Text, pQuestionData.Order, eQuestionType.Stars)
        {
            NumberOfStars = pNumberOfstars;
        }
    }
}
