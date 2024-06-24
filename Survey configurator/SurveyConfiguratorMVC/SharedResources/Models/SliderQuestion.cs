using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Models
{
    public class SliderQuestion : Question
    {
        public int StartValue { get; set; }
        public int EndValue { get; set; }
        public string StartValueCaption { get; set; }
        public string EndValueCaption { get; set; }

        public SliderQuestion(int pId, string pText, int pOrder, int pStartValue = 0, int pEndValue = 100, string pStartCaption = "Min", string pEndCaption = "Max")
            : base(pId, pText, pOrder, eQuestionType.Slider)
        {
            StartValue = pStartValue;
            EndValue = pEndValue;
            StartValueCaption = pStartCaption;
            EndValueCaption = pEndCaption;
        }

        public SliderQuestion(Question pQuestionData, int pStartValue = 0, int pEndValue = 100, string pStartCaption = "Min", string pEndCaption = "Max")
            : base(pQuestionData.Id, pQuestionData.Text, pQuestionData.Order, eQuestionType.Slider)
        {
            StartValue = pStartValue;
            EndValue = pEndValue;
            StartValueCaption = pStartCaption;
            EndValueCaption = pEndCaption;
        }
    }
}
