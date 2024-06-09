using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SurveyConfiguratorWeb.Models
{
    public class SmileyQuestionOptions
    {
        [Required(ErrorMessage = "The number of smiley faces is required")]
        [Range(3, 5, ErrorMessage = "The number of smiley faces must be between 3 and 5")]
        [DisplayName("Number of smiley faces")]
        public int NumberOfSmileyFaces { get; set; }

    }
}