using QuestionDB;
using SharedResources.Models;
using SharedResources;
using System;

namespace QuestionServices
{
    public class AuthenticationServices
    {
        /// <summary>
        /// this class is the one responsible for the communication between the UI layer
        /// and the database in the context of Authentication
        /// </summary>


        /// <summary>
        /// sends the received token id to the database layer
        /// to add it as an invalid token Id
        /// </summary>
        /// <param name="pTokenId">the Id of the token to add to database</param>
        /// <returns>OperationResult to indicate whether the addition operation was successful</returns>
        public static OperationResult AddToken(string pTokenId)
        {
            try
            {
                if(pTokenId != null) 
                { 
                    //add the token to database
                    OperationResult tAddTokenResult = AuthenticationDB.AddTokenId(pTokenId);
                    //on successful question addition to Database add it to the Questions List
                    return tAddTokenResult;
                }
                return new OperationResult(GlobalStrings.NullValueErrorTitle, GlobalStrings.NullValueError);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }
        }

        /// <summary>
        /// sends the received token id to the database layer
        /// to check if it's stored in the database
        /// </summary>
        /// <param name="pTokenId">the Id of the token to check its validity</param>
        /// <returns>OperationResult to indicate whether the token was found</returns>
        public static OperationResult CheckTokenValidity(string pTokenId)
        {
            try
            {
                if (pTokenId != null)
                {
                    OperationResult tIsTokenValid  = AuthenticationDB.CheckTokenExpire(pTokenId);
                    return tIsTokenValid;
                }
                return new OperationResult(GlobalStrings.NullValueErrorTitle, GlobalStrings.NullValueError);
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);

            }
        }
    }
}
