using SharedResources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SurveyConfiguratorWeb.Models
{
    public class StarsQuestionOptions
    {
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