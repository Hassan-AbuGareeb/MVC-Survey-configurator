using System;

namespace SharedResources
{
    public class OperationResult
    {
        /// <summary>
        /// this class is used to implement the Operation result pattern where
        /// each request or function to anywhere outside of the application like
        /// a database or API would return a result of failure or success 
        /// and the data can be changed by refernece.
        /// </summary>

        public string mError { get; set; }
        public string mErrorMessage { get; set; }
        public bool IsSuccess { get; set; }

        //successful operation
        public OperationResult()
        {
            try
            {
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }

        //failed operation with error type and message
        public OperationResult(string pError, string pErrorMessage)
        {
            try
            {
                IsSuccess = false;
                mErrorMessage = pErrorMessage;
                mError = pError;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }
    }
}
