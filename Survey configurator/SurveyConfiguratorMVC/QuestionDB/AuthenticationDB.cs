using System;
using System.Collections.Generic;
using SharedResources.Models;
using Microsoft.Data.SqlClient;
using SharedResources;
using System.Data.Common;
using System.Data;
using System.Diagnostics;


namespace QuestionDB
{
    public class AuthenticationDB
    {
        //constants
        //RefreshTokens table constants
        private const string cTokensTableName = "RefreshTokens";
        private const string cIdColumn = "Id";
        private const string cExpireDateColumn = "ExpireDate";

        private AuthenticationDB() { }

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
                                $"USE Authentication_DB INSERT INTO {cTokensTableName} ([{cIdColumn}], [{cExpireDateColumn}]) " +
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
    }
}
