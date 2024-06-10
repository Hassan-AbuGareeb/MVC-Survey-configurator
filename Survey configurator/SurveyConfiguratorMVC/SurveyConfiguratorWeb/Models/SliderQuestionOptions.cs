using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.RightsManagement;
namespace SurveyConfiguratorWeb.Models
{
    public class SliderQuestionOptions
    {


        [Required(ErrorMessage = "The Start value is required")]
        [Range(0, 100, ErrorMessage = "The value of the Start value must be between 0 and 100")]
        [DisplayName("Start value")]
        public int StartValue { get; set; }

        [Required(ErrorMessage = "The End Vlaue is required")]
        [Range(0, 100, ErrorMessage = "The value of the end value must be between 0 and 100")]
        [DisplayName("End value")]
        public int EndValue { get; set; }

        [Required(ErrorMessage = "The start value caption is required")]
        [DisplayName("Start value caption")]
        public string StartValueCaption { get; set; }

        [Required(ErrorMessage = "The end value caption is required")]
        [DisplayName("End value caption")]
        public string EndValueCaption { get; set; }

        public SliderQuestionOptions()
        {

        }

        public SliderQuestionOptions(int startValue, int endValue, string startValueCaption, string endValueCaption)
        {
            StartValue = startValue;
            EndValue = endValue;
            StartValueCaption = startValueCaption;
            EndValueCaption = endValueCaption;
        }
    }
}