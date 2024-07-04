using SharedResources;
using System;
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
        [Range(SharedData.cMinNumberOfStars, SharedData.cMaxNumberOfStars, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "StarsNumberError")]
        public int NumberOfStars { get; set; }

        public StarsQuestionOptions() { }

        public StarsQuestionOptions(int numberOfStars) 
        {
            try 
            { 
                NumberOfStars = numberOfStars;
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }
    }
}