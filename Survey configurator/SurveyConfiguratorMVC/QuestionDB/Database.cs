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
    public class Database
    {
        /// <summary>
        /// this class is concerned with communication with the database,
        /// it is the interface between the logic layer(QuestionsOperations) class
        /// and the database
        /// </summary>


        public static string mConnectionString = "";

        //constants
        //Question table constants
        private const string cQuestionsTableName = "Question";
        private const string cIdColumn = "Id";
        private const string cTextColumn = "Text";
        private const string cOrderColumn = "Order";
        private const string cTypeColumn = "Type";
        //specific types constants
        //Stars table
        private const string cNumberOfStarsColumn = "NumberOfStars";
        //Smiley table
        private const string cNumberOfFacesColumn = "NumberOfFaces";
        //Slider table
        private const string cStartValueColumn = "StartValue";
        private const string cEndValueColumn = "EndValue";
        private const string cStartValueCaptionColumn = "StartValueCaption";
        private const string cEndValueCaptionColumn = "EndValueCaption";


        private Database() { }
        #region class main functions

        /// <summary>
        /// this function test the connection with the database
        /// </summary>
        /// <returns>OperationResult to indicate whether a connection can be established to the database</returns>
        public static OperationResult TestDataBaseConnection()
        {
            try
            {
                SqlConnection tConn = new SqlConnection(mConnectionString);
                tConn.Open();
                tConn.Close();
                return new OperationResult();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.SqlErrorTitle, GlobalStrings.SqlError);
            }
        }

        /// <summary>
        /// gets the all the questions general data from the database and assigns it to the
        /// pQuestionsList param if the operation was successful
        /// </summary>
        /// <param name="pQuestionsList">gets filled with data obtained from the database</param>
        /// <returns>OperationResult to indicate whether data was fetched from database</returns>
        public static OperationResult GetQuestionsFromDB(ref List<Question> pQuestionsList)
        {
            try
            {
                using (SqlConnection tConn = new SqlConnection(mConnectionString))
                {
                    tConn.Open();
                    SqlCommand tGetQuestionsDataCmd = new SqlCommand($"SELECT * FROM [{cQuestionsTableName}]", tConn);

                    DbDataReader tReader = tGetQuestionsDataCmd.ExecuteReader(CommandBehavior.CloseConnection);
                    //iterate over each row in the Reader and add it to the questions list
                    while (tReader.Read())
                    {
                        pQuestionsList.Add(new Question((int)tReader[cIdColumn], tReader[cTextColumn].ToString(),
                            (int)tReader[cOrderColumn], (eQuestionType)tReader[cTypeColumn]));
                    }
                    tReader.Close();
                }
                return new OperationResult();
            }
            catch (SqlException ex)
            {
                Debug.Write(ex.ToString());
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.SqlErrorTitle, GlobalStrings.SqlError);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }
        }

        /// <summary>
        /// gets the specific data of a question from the database, and fills the pQuestionSpecificData param
        /// with the fetched data only if the operation succeeds
        /// </summary>
        /// <param name="pQuestionData">general question data</param>
        /// <param name="pQuestionSpecificData">to be filled with fetched data from the database</param>
        /// <returns>OperationResult to indicate whether data was fetched from database</returns>
        public static OperationResult GetQuestionSpecificDataFromDB(Question pQuestionData, ref Question pQuestionSpecificData)
        {
            try
            {
                using (SqlConnection tConn = new SqlConnection(mConnectionString))
                {
                    tConn.Open();
                    SqlCommand tGetQuestionSpecificData = new SqlCommand(
                        $"SELECT * FROM " +
                        $"[{pQuestionData.Type}] " +
                        $"WHERE [{cIdColumn}] = @{cIdColumn}", tConn);
                    tGetQuestionSpecificData.Parameters.Add(new SqlParameter($"@{cIdColumn}", pQuestionData.Id));

                    DbDataReader tReader = tGetQuestionSpecificData.ExecuteReader(CommandBehavior.CloseConnection);
                    while (tReader.Read())
                    {
                        switch (pQuestionData.Type)
                        {
                            case eQuestionType.Stars:
                                pQuestionSpecificData = new StarsQuestion(pQuestionData, (int)tReader[cNumberOfStarsColumn]);
                                break;
                            case eQuestionType.Smiley:
                                pQuestionSpecificData = new SmileyQuestion(pQuestionData, (int)tReader[cNumberOfFacesColumn]);
                                break;
                            case eQuestionType.Slider:
                                pQuestionSpecificData = new SliderQuestion(pQuestionData,
                                    (int)tReader[cStartValueColumn], (int)tReader[cEndValueColumn],
                                    tReader[cStartValueCaptionColumn].ToString(), tReader[cEndValueCaptionColumn].ToString());
                                break;

                        }
                    }
                    tReader.Close();
                    return new OperationResult();
                }
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }
        }

        /// <summary>
        /// gets the question data and create a row for that question in the 
        /// database and gets back the id generated by the database,
        /// choosing this way of generating ids is to insure consistency of 
        /// generated ids when creating questions from different sources.
        /// </summary>
        /// <param name="pQuestionData"></param>
        /// <returns>OperationResult to indicate whether data was added to the database</returns>
        public static OperationResult AddQuestionToDB(ref Question pQuestionData)
        {
            try
            {
                //create db connection
                using (SqlConnection tConn = new SqlConnection(mConnectionString))
                {
                    tConn.Open();
                    using (SqlTransaction tTransaction = tConn.BeginTransaction())
                    {
                        try
                        {
                            //insert question data in the question table
                            SqlCommand tInsertQuestionCmd = new SqlCommand(
                                $"INSERT INTO {cQuestionsTableName} ([{cTextColumn}], [{cOrderColumn}], [{cTypeColumn}]) " +
                                $"OUTPUT INSERTED.Id " +
                                $"VALUES (@{cTextColumn}, @{cOrderColumn}, @{cTypeColumn})",
                                tConn, tTransaction);
                            tInsertQuestionCmd.Parameters.AddRange(new SqlParameter[] {
                                new SqlParameter($"@{cTextColumn}", pQuestionData.Text),
                                new SqlParameter($"@{cOrderColumn}", pQuestionData.Order),
                                new SqlParameter($"@{cTypeColumn}", pQuestionData.Type)
                            });

                            //insert the row data to the question and return the id of the created question
                            int tQuestionId = (int)tInsertQuestionCmd.ExecuteScalar();

                            //add the question Id to the quesiton data
                            pQuestionData.Id = tQuestionId;

                            //for each type of question downcast the question to its specific type and obtain its properties
                            SqlCommand tInsertQuestionTypeCmd = null;
                            switch (pQuestionData.Type)
                            {
                                case eQuestionType.Stars:
                                    tInsertQuestionTypeCmd = GetAddStarsCommand(tQuestionId, (StarsQuestion)pQuestionData);
                                    break;
                                case eQuestionType.Smiley:
                                    tInsertQuestionTypeCmd = GetAddSmileyCommand(tQuestionId, (SmileyQuestion)pQuestionData);
                                    break;
                                case eQuestionType.Slider:
                                    tInsertQuestionTypeCmd = GetAddSliderCommand(tQuestionId, (SliderQuestion)pQuestionData);
                                    break;
                            }
                            //add parameters
                            tInsertQuestionTypeCmd.Connection = tConn;
                            tInsertQuestionTypeCmd.Transaction = tTransaction;
                            tInsertQuestionTypeCmd.ExecuteNonQuery();
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
        /// updates recieved question data, the original data type is required as to
        /// check whether the question type has changed, because in that case the row
        /// of its original type has to be deleted from the database to ensure that 
        /// there isn't a question associated with more than one type table in the database.
        /// </summary>
        /// <param name="pOriginalQuestionType"></param>
        /// <param name="pUpdatedQuestionData"></param>
        /// <returns>OperationResult to indicate whether data updated on the database</returns>
        public static OperationResult UpdateQuestionOnDB(eQuestionType pOriginalQuestionType, Question pUpdatedQuestionData)
        {
            try
            {
                using (SqlConnection tConn = new SqlConnection(mConnectionString))
                {
                    tConn.Open();
                    using (SqlTransaction tTransaction = tConn.BeginTransaction())
                    {
                        try
                        {
                            if (pUpdatedQuestionData.Type.Equals(pOriginalQuestionType))
                            {
                                //type of question wasn't  changed
                                //update the specific details
                                SqlCommand tUpdateQuestionSpecificDataCmd = null;
                                switch (pOriginalQuestionType)
                                {
                                    case eQuestionType.Stars:
                                        tUpdateQuestionSpecificDataCmd = GetUpdateStarsCommand(pUpdatedQuestionData.Id, (StarsQuestion)pUpdatedQuestionData);
                                        break;
                                    case eQuestionType.Smiley:
                                        tUpdateQuestionSpecificDataCmd = GetUpdateSmileyCommand(pUpdatedQuestionData.Id, (SmileyQuestion)pUpdatedQuestionData);
                                        break;
                                    case eQuestionType.Slider:
                                        tUpdateQuestionSpecificDataCmd = GetUpdateSliderCommand(pUpdatedQuestionData.Id, (SliderQuestion)pUpdatedQuestionData);
                                        break;

                                }

                                tUpdateQuestionSpecificDataCmd.Connection = tConn;
                                tUpdateQuestionSpecificDataCmd.Transaction = tTransaction;
                                tUpdateQuestionSpecificDataCmd.ExecuteNonQuery();
                            }
                            else
                            {
                                //type of question changed
                                //delete the questions specific old data first
                                SqlCommand tDeleteSpecificQuestionDataCmd = new SqlCommand(
                                    $"DELETE FROM {pOriginalQuestionType} " +
                                    $"WHERE [{cIdColumn}] = @{cIdColumn}",
                                    tConn, tTransaction);
                                tDeleteSpecificQuestionDataCmd.Parameters.Add(new SqlParameter($@"{cIdColumn}", pUpdatedQuestionData.Id));

                                //create a new row in the specific question type table
                                SqlCommand tInsertQuestionTypeCmd = null;

                                //for each type of question downcast the question to its specific type
                                switch (pUpdatedQuestionData.Type)
                                {
                                    case eQuestionType.Stars:
                                        tInsertQuestionTypeCmd = GetAddStarsCommand(pUpdatedQuestionData.Id, (StarsQuestion)pUpdatedQuestionData);
                                        break;
                                    case eQuestionType.Smiley:
                                        tInsertQuestionTypeCmd = GetAddSmileyCommand(pUpdatedQuestionData.Id, (SmileyQuestion)pUpdatedQuestionData);
                                        break;
                                    case eQuestionType.Slider:
                                        tInsertQuestionTypeCmd = GetAddSliderCommand(pUpdatedQuestionData.Id, (SliderQuestion)pUpdatedQuestionData);
                                        break;

                                }

                                tInsertQuestionTypeCmd.Connection = tConn;
                                tInsertQuestionTypeCmd.Transaction = tTransaction;
                                //exectue commands on database
                                tDeleteSpecificQuestionDataCmd.ExecuteNonQuery();
                                tInsertQuestionTypeCmd.ExecuteNonQuery();
                            }

                            //set the data type based on whether it changed or not
                            int pUpdatedQuestionType = pOriginalQuestionType == pUpdatedQuestionData.Type ? (int)pOriginalQuestionType : (int)pUpdatedQuestionData.Type;

                            //update the general data of the question after changing the specific data
                            SqlCommand tUpdateQuestionDataCmd = new SqlCommand(
                                $"UPDATE {cQuestionsTableName} SET" +
                                $" [{cOrderColumn}] = @{cOrderColumn}," +
                                $" [{cTextColumn}] = @{cTextColumn}, " +
                                $" [{cTypeColumn}] = @{cTypeColumn}" +
                                $" WHERE [{cIdColumn}] = @{cIdColumn}",
                                   tConn, tTransaction);
                            //add parameters to the query
                            tUpdateQuestionDataCmd.Parameters.AddRange(
                            new SqlParameter[] {
                                new SqlParameter($"@{cIdColumn}", pUpdatedQuestionData.Id),
                                new SqlParameter($"@{cTextColumn}", pUpdatedQuestionData.Text),
                                new SqlParameter($"@{cOrderColumn}", pUpdatedQuestionData.Order),
                                new SqlParameter($"@{cTypeColumn}", pUpdatedQuestionType)
                            });
                            tUpdateQuestionDataCmd.ExecuteNonQuery();
                            tTransaction.Commit();
                            return new OperationResult();
                        }
                        catch (Exception ex)
                        {
                            UtilityMethods.LogError(ex);
                            tTransaction.Rollback();
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
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }
        }

        /// <summary>
        /// delete the selected question/questions from the database, starts with deleting each
        /// questions specific type row first then deleting its general data because of a primary key
        /// forign key relationship constraints.
        /// </summary>
        /// <param name="pSelectedQuestions">list of questions objects to delete</param>
        /// <returns>OperationResult to indicate whether data was deleted from database</returns>
        public static OperationResult DeleteQuestionFromDB(List<Question> pSelectedQuestions)
        {
            try
            {
                using (SqlConnection tConn = new SqlConnection(mConnectionString))
                {
                    tConn.Open();
                    using (SqlTransaction tTransaction = tConn.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand tDeleteQuestionsCmd = tConn.CreateCommand();
                            tDeleteQuestionsCmd.CommandType = CommandType.Text;
                            tDeleteQuestionsCmd.Transaction = tTransaction;
                            //delete the specific details of the question type
                            for (int i = 0; i < pSelectedQuestions.Count; i++)
                            {
                                Question tCurrentQuestion = pSelectedQuestions[i];
                                tDeleteQuestionsCmd.CommandText =
                                    $"DELETE FROM {tCurrentQuestion.Type} " +
                                    $"WHERE [{cIdColumn}] = @{cIdColumn}";
                                tDeleteQuestionsCmd.Parameters.Add(new SqlParameter($"@{cIdColumn}", tCurrentQuestion.Id));
                                tDeleteQuestionsCmd.ExecuteNonQuery();
                                tDeleteQuestionsCmd.Parameters.Clear();
                            }

                            //delete question from database
                            for (int i = 0; i < pSelectedQuestions.Count; i++)
                            {
                                Question tCurrentQuestion = pSelectedQuestions[i];
                                tDeleteQuestionsCmd.CommandText =
                                    $"DELETE FROM {cQuestionsTableName} " +
                                    $"WHERE [{cIdColumn}] = @{cIdColumn}";
                                tDeleteQuestionsCmd.Parameters.Add(new SqlParameter($"@{cIdColumn}", tCurrentQuestion.Id));
                                tDeleteQuestionsCmd.ExecuteNonQuery();
                                tDeleteQuestionsCmd.Parameters.Clear();
                            }
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
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }
        }
        #endregion

        #region class utility functions
        /// <summary>
        /// the following functions are responsible for creating 
        /// SqlCommands to create rows for the speicfic questions types
        /// in the database
        /// </summary>

        private static SqlCommand GetAddStarsCommand(int pQuestionId, StarsQuestion pStarsQuestionData)
        {
            try
            {
                SqlCommand tInsertQuestionTypeCmd = new SqlCommand(
                    $"INSERT INTO {eQuestionType.Stars} " +
                    $"([{cIdColumn}], [{cNumberOfStarsColumn}]) " +
                    $"VALUES ( @{cIdColumn}, @{cNumberOfStarsColumn} )");

                tInsertQuestionTypeCmd.Parameters.AddRange(new SqlParameter[] {
                    new SqlParameter($"@{cIdColumn}", pQuestionId),
                    new SqlParameter($"@{cNumberOfStarsColumn}", pStarsQuestionData.NumberOfStars)
                });
                return tInsertQuestionTypeCmd;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }

        }
        private static SqlCommand GetAddSmileyCommand(int pQuestionId, SmileyQuestion pSmileyQuestionData)
        {
            try
            {
                SqlCommand tInsertQuestionTypeCmd = new SqlCommand(
                    $"INSERT INTO {eQuestionType.Smiley} " +
                    $"([{cIdColumn}], [{cNumberOfFacesColumn}]) " +
                    $"VALUES ( @{cIdColumn}, @{cNumberOfFacesColumn} )");

                tInsertQuestionTypeCmd.Parameters.AddRange(new SqlParameter[] {
                    new SqlParameter($"@{cIdColumn}", pQuestionId),
                    new SqlParameter($"@{cNumberOfFacesColumn}", pSmileyQuestionData.NumberOfSmileyFaces)
                });
                return tInsertQuestionTypeCmd;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }

        }
        private static SqlCommand GetAddSliderCommand(int pQuestionId, SliderQuestion pSliderQuestionData)
        {
            try
            {
                SqlCommand tInsertQuestionTypeCmd = new SqlCommand(
                    $"INSERT INTO {eQuestionType.Slider} " +
                    $"([{cIdColumn}], [{cStartValueColumn}],[{cEndValueColumn}],[{cStartValueCaptionColumn}],[{cEndValueCaptionColumn}]) " +
                    $"VALUES (@{cIdColumn}, @{cStartValueColumn}, @{cEndValueColumn}, @{cStartValueCaptionColumn}, @{cEndValueCaptionColumn})");

                tInsertQuestionTypeCmd.Parameters.AddRange(new SqlParameter[] {
                    new SqlParameter($"@{cIdColumn}", pQuestionId),
                    new SqlParameter($"@{cStartValueColumn}", pSliderQuestionData.StartValue),
                    new SqlParameter($"@{cEndValueColumn}", pSliderQuestionData.EndValue),
                    new SqlParameter($"@{cStartValueCaptionColumn}", pSliderQuestionData.StartValueCaption),
                    new SqlParameter($"@{cEndValueCaptionColumn}", pSliderQuestionData.EndValueCaption)
                });
                return tInsertQuestionTypeCmd;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// the following functions are responsible for creating 
        /// SqlCommands to update rows for the speicfic questions types
        /// in the database
        /// </summary>


        private static SqlCommand GetUpdateStarsCommand(int pQuestionId, StarsQuestion pStarsQuestionData)
        {
            try
            {
                SqlCommand tUpdateQuestionSpecificDataCmd = new SqlCommand(
                    $"UPDATE {eQuestionType.Stars} SET " +
                    $"[{cNumberOfStarsColumn}] = @{cNumberOfStarsColumn} " +
                    $"WHERE [{cIdColumn}] = @{cIdColumn}");

                tUpdateQuestionSpecificDataCmd.Parameters.AddRange(new SqlParameter[] {
                    new SqlParameter($"@{cIdColumn}", pQuestionId),
                    new SqlParameter($"@{cNumberOfStarsColumn}", pStarsQuestionData.NumberOfStars)
                });
                return tUpdateQuestionSpecificDataCmd;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }

        }
        private static SqlCommand GetUpdateSmileyCommand(int pQuestionId, SmileyQuestion pSmileyQuestionData)
        {
            try
            {
                SqlCommand tUpdateQuestionSpecificDataCmd = new SqlCommand(
                    $"UPDATE {eQuestionType.Smiley} SET " +
                    $"[{cNumberOfFacesColumn}] = @{cNumberOfFacesColumn} " +
                    $"WHERE [{cIdColumn}] = @{cIdColumn}");

                tUpdateQuestionSpecificDataCmd.Parameters.AddRange(new SqlParameter[] {
                    new SqlParameter($"@{cIdColumn}", pQuestionId),
                    new SqlParameter($"@{cNumberOfFacesColumn}", pSmileyQuestionData.NumberOfSmileyFaces)
                });
                return tUpdateQuestionSpecificDataCmd;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }

        }
        private static SqlCommand GetUpdateSliderCommand(int pQuestionId, SliderQuestion pStarsQuestionData)
        {
            try
            {
                SqlCommand tUpdateQuestionSpecificDataCmd = new SqlCommand(
                    $"UPDATE {eQuestionType.Slider} SET " +
                    $"[{cStartValueColumn}] = @{cStartValueColumn}, " +
                    $"[{cEndValueColumn}] = @{cEndValueColumn}, " +
                    $"[{cStartValueCaptionColumn}] = @{cStartValueCaptionColumn}, " +
                    $"[{cEndValueCaptionColumn}] = @{cEndValueCaptionColumn} " +
                    $"WHERE [{cIdColumn}] = @{cIdColumn}");

                tUpdateQuestionSpecificDataCmd.Parameters.AddRange(new SqlParameter[] {
                    new SqlParameter($"@{cIdColumn}", pQuestionId),
                    new SqlParameter($"@{cStartValueColumn}", pStarsQuestionData.StartValue),
                    new SqlParameter($"@{cEndValueColumn}", pStarsQuestionData.EndValue),
                    new SqlParameter($"@{cStartValueCaptionColumn}", pStarsQuestionData.StartValueCaption),
                    new SqlParameter($"@{cEndValueCaptionColumn}", pStarsQuestionData.EndValueCaption)
                });
                return tUpdateQuestionSpecificDataCmd;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// gets the checksum value of the database and put it in the received param.
        /// </summary>
        /// <param name="pChecksum">filled with value obtained from database</param>
        /// <returns>OperationResult to indicate whether the checksum was successfully obtained</returns>
        public static OperationResult GetChecksum(ref long pChecksum)
        {
            try
            {
                using (SqlConnection tConn = new SqlConnection(mConnectionString))
                {
                    tConn.Open();
                    SqlCommand tGetChecksum = new SqlCommand($"SELECT CHECKSUM_AGG(BINARY_CHECKSUM(*)) FROM {cQuestionsTableName} WITH (NOLOCK)", tConn);
                    var tChecksum = tGetChecksum.ExecuteScalar();
                    //database is empty
                    if (DBNull.Value.Equals(tChecksum))
                    {
                        pChecksum = 0;
                        return new OperationResult();
                    }
                    pChecksum = (int)tChecksum;
                    return new OperationResult();
                }
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }
        }
        #endregion
    }
}
