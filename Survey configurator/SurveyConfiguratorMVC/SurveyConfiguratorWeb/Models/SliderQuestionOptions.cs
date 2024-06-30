using SharedResources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.RightsManagement;
namespace SurveyConfiguratorWeb.Models
{
    public class SliderQuestionOptions
    {
        /// <summary>
        /// view model for the Slider question object partial view
        /// with the appropriate data annotations to help 
        /// with the validation
        /// the errors messages are fetched from the resource file to 
        /// enable the localization 
        /// </summary>


        [Required(ErrorMessageResourceType =typeof(GlobalStrings), ErrorMessageResourceName= "SliderStartValueRequiredError")]
        [Range(SharedData.cMinStartValue, SharedData.cMaxStartValue, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "SliderStartValueNumberError")]
        [DisplayName("Start value")]
        public int StartValue { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "SliderEndValueRequiredError")]
        [Range(SharedData.cMinEndValue, SharedData.cMaxEndValue, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "SliderEndValueNumberError")]
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
            try 
            { 
            StartValue = startValue;
            EndValue = endValue;
            StartValueCaption = startValueCaption;
            EndValueCaption = endValueCaption;
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }
    }
}