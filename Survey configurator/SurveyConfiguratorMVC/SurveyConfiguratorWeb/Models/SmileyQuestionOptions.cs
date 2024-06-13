using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using SharedResources;

namespace SurveyConfiguratorWeb.Models
{
    public class SmileyQuestionOptions
    {
        [Required(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "SmileyRequiredError")]
        [Range(2, 5, ErrorMessageResourceType =typeof(GlobalStrings), ErrorMessageResourceName = "SmileyNumberError")]
        [DisplayName("Number of smiley faces")]
        public int NumberOfSmileyFaces { get; set; }

        public SmileyQuestionOptions()
        {

        }

        public SmileyQuestionOptions(int numberOfSmileyFaces)
        {
            NumberOfSmileyFaces = numberOfSmileyFaces;
        }
    }
}