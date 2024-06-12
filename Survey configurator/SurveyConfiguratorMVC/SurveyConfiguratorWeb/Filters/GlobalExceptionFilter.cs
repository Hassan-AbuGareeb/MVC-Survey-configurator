using SharedResources;
using System;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Filters
{
    public class GlobalExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext pfilterContext)
        {
            //log error redirect 
            Exception tException = pfilterContext.Exception;
            UtilityMethods.LogError(tException);

            pfilterContext.ExceptionHandled = true;

            //assign the error view to the result to show it

            ViewResult tErrorResult = new ViewResult();
            tErrorResult.ViewName = "Error";

            pfilterContext.Result = tErrorResult;
        }
    }
}