using SharedResources;
using SurveyConfiguratorWeb.ConstantsAndMethods;
using System;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Filters
{
    public class GlobalExceptionFilter : FilterAttribute, IExceptionFilter
    {
        /// <summary>
        /// Custom filter attribute that acts as a global exception handler
        /// logs exceptions and redirect to an error page
        /// </summary>




        /// <summary>
        /// overriden method which is called on any unhandled exception
        /// in this context.
        /// </summary>
        /// <param name="pfilterContext"></param>
        /// 
        public void OnException(ExceptionContext pfilterContext)
        {
            try { 
            //log error redirect 
            Exception tException = pfilterContext.Exception;
            UtilityMethods.LogError(tException);

            pfilterContext.ExceptionHandled = true;

            //assign the error view to the result to show it
            ViewResult tErrorResult = new ViewResult { ViewName=SharedConstants.cErrorController};
            pfilterContext.Result = tErrorResult;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }
    }
}