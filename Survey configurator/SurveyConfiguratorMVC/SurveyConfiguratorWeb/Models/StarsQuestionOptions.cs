using SharedResources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SurveyConfiguratorWeb.Models
{
    public class StarsQuestionOptions
    {
        /// <summary>
        /// view model for the Stars question object partial view
        /// with the appropriate data annotations to help 
        /// with the validation
        /// the errors messages are fetched from the resource file to 
        /// enable the localization 
        /// </summary>

        [Required(ErrorMessageResourceType =typeof(GlobalStrings) ,ErrorMessageResourceName="StarsRequiredError")]
        [Range(1,10, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "StarsNumberError")]
        [DisplayName("Number of stars")]
        public int NumberOfStars { get; set; }

        public StarsQuestionOptions() { }

        public StarsQuestionOptions(int numberOfStars) 
        {
            NumberOfStars = numberOfStars;
        }
    }
}