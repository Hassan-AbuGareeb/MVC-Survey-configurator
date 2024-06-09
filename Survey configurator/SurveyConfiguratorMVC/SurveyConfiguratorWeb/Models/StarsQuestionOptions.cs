using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SurveyConfiguratorWeb.Models
{
    public class StarsQuestionOptions
    {
        [Required(ErrorMessage ="The number of stars is required")]
        [Range(3,10,ErrorMessage ="The number of stars must be between 3 and 10")]
        [DisplayName("Number of stars")]
        public int NumberOfStars { get; set; }
    }
}