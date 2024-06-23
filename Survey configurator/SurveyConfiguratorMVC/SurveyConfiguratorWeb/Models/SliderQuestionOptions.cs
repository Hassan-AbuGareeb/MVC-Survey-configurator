using SharedResources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.RightsManagement;
namespace SurveyConfiguratorWeb.Models
{
    public class SliderQuestionOptions
    {


        [Required(ErrorMessageResourceType =typeof(GlobalStrings), ErrorMessageResourceName= "SliderStartValueRequiredError")]
        [Range(0, 100, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "SliderStartValueNumberError")]
        [DisplayName("Start value")]
        public int StartValue { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "SliderEndValueRequiredError")]
        [Range(0, 100, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "SliderEndValueNumberError")]
        [DisplayName("End value")]
        public int EndValue { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "SliderStartCaptionRequiredError")]
        [DisplayName("Start value caption")]
        public string StartValueCaption { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "SliderEndCaptionRequiredError")]
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