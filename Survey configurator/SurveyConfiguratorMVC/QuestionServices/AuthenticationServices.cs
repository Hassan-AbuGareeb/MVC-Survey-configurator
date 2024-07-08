using QuestionDB;
using SharedResources.Models;
using SharedResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionServices
{
    public class AuthenticationServices
    {
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
    }
}
