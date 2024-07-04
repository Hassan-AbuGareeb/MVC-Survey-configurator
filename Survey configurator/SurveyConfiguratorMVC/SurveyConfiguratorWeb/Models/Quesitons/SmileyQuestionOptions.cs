using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using SharedResources;
using System;

namespace SurveyConfiguratorWeb.Models
{
    public class SmileyQuestionOptions
    {
        /// <summary>
        /// view model for the Smiley question object partial view
        /// with the appropriate data annotations to help 
        /// with the validation
        /// the errors messages are fetched from the resource file to 
        /// enable the localization 
        /// </summary>

        [Required(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "SmileyRequiredError")]
        [Range(SharedData.cMinNumberOfSmileyFaces, SharedData.cMaxNumberOfSmileyFaces,
            ErrorMessageResourceType =typeof(GlobalStrings), ErrorMessageResourceName = "SmileyNumberError")]
        [DisplayName("Number of smiley faces")]
        public int NumberOfSmileyFaces { get; set; }

        public SmileyQuestionOptions()
        {

        }

        public SmileyQuestionOptions(int numberOfSmileyFaces)
        {
            try { 
            NumberOfSmileyFaces = numberOfSmileyFaces;
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }
    }
}