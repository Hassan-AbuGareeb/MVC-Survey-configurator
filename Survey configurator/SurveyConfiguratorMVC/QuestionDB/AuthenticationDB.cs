using System;
using Microsoft.Data.SqlClient;
using SharedResources;


namespace QuestionDB
{
    public class AuthenticationDB
    {
        /// <summary>
        /// this class is responsible for communicating with the 
        /// authentication database.
        /// </summary>


        //constants
        //RefreshTokens table constants
        private const string cAuthenticationDatabase = "Authentication_DB";
        private const string cTokensTableName = "RefreshTokens";
        private const string cIdColumn = "Id";
        private const string cExpireDateColumn = "ExpireDate";

        /// <summary>
        /// prevent the class from being instantiated
        /// </summary>
        private AuthenticationDB() { }

        /// <summary>
        /// Add the received token id to the Refreshtokens table,
        /// this table contains invalid tokens so any token contained
        /// in this table shouldn't be accepted as a valid token.
        /// also an expiration date is sent with the token id for the sql
        /// server to delete the token from the database after that certian date
        /// </summary>
        /// <param name="pTokenId">Id of invalid token</param>
        /// <returns>operationResult object indicating the success of the addition operation</returns>
        public static OperationResult AddTokenId(string pTokenId)
        {
            try 
            { 
                using (SqlConnection tConn = new SqlConnection(Database.mConnectionString))
                {
                    tConn.Open();
                    using (SqlTransaction tTransaction = tConn.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand tInsertIdCommand = new SqlCommand (
                                $"USE {cAuthenticationDatabase} INSERT INTO {cTokensTableName} ([{cIdColumn}], [{cExpireDateColumn}]) " +
                                $"VALUES (@{cIdColumn}, @{cExpireDateColumn})",
                                tConn, tTransaction);
                            //add parameters
                            tInsertIdCommand.Parameters.Add(new SqlParameter($"@{cIdColumn}", pTokenId));
                            tInsertIdCommand.Parameters.Add(new SqlParameter($"@{cExpireDateColumn}", DateTime.Now.AddDays(SharedData.cRefreshTokenExpireTimeInDays)));

                            //execute command
                            tInsertIdCommand.ExecuteNonQuery();

                            //commit transaction
                            tTransaction.Commit();
                            return new OperationResult();
                        }
                        catch (Exception ex)
                        {
                            tTransaction.Rollback();
                            UtilityMethods.LogError(ex);
                            return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.SqlErrorTitle, GlobalStrings.SqlError);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError); ;
            }
        }

        /// <summary>
        /// this function checks whether the token is stored in the database,
        /// if it's found then the token is invalid and return failed operation result
        /// else the token is still valid return a successful operation result
        /// </summary>
        /// <param name="pTokenId">Id of received token</param>
        /// <returns>operationResult object indicating whether the token was found or not</returns>
        public static OperationResult CheckTokenExpire(string pTokenId)
        {
            try
            {
                using (SqlConnection tConn = new SqlConnection(Database.mConnectionString))
                {
                    tConn.Open();
                    SqlCommand tInsertIdCommand = new SqlCommand(
                              $"USE {cAuthenticationDatabase} SELECT {cIdColumn} " +
                              $"FROM {cTokensTableName} " +
                              $"WHERE {cIdColumn} = @{cIdColumn}",
                              tConn);
                    //add parameters
                    tInsertIdCommand.Parameters.Add(new SqlParameter($"@{cIdColumn}", pTokenId));

                    //execute command
                    var tTokenId = (string)tInsertIdCommand.ExecuteScalar();

                    if (string.IsNullOrEmpty(tTokenId))
                    {
                        //token is not in the table, still valid
                        return new OperationResult();
                    }
                    //token found, Invalid
                    return new OperationResult(GlobalStrings.TokenInvalidErrorTitle, GlobalStrings.TokenInvalidError);
                }
            }
            catch (SqlException ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.SqlErrorTitle, GlobalStrings.SqlError);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError); ;
            }
        }
    }
}
