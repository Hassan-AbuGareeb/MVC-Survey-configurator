using QuestionDB;
using SharedResources.Models;
using SharedResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace QuestionServices
{
    public class QuestionOperations
    {
        /// <summary>
        /// this class is the one responsible for the communication between the UI layer
        /// and the database, it gets the requested data from the database and applies 
        /// any kind of logic to it before sending it to the UI layer.
        /// it's also responsible for doing other operations such as mointoring the database
        /// for any change and informing the UI layer when it occurs, and more operations mentioned below.
        /// </summary>


        //event handlers
        //event handler for any change that happens to the database from any source
        public static event EventHandler DataBaseChangedEvent;
        //event handler for when the database stops responding

        public static event EventHandler DataBaseNotConnectedEvent;
        //contsants
        private const int cDatabaseReconnectMaxAttempts = 3;
        public const string cConnectionStringFileName = "\\connectionSettings.json";

        //class members
        //changed to true when the user is performing adding, updating or deleting operation
        public static bool mOperationOngoing = false;
        public static string mFilePath = Directory.GetCurrentDirectory() + cConnectionStringFileName;

        //a list to temporarily contain the questions data fetched from the database, 
        //and acts as a data source for the UI to faciltate data transfer and fetching.
        public static List<Question> mQuestionsList = new List<Question>();

        private QuestionOperations()
        {
        }

        #region class main functions
        /// <summary>
        /// this function is responsible for fetching the General Question info (Id, text, order, type)
        /// from the database, it fills the QuestionList field with the obtained questions data.
        /// </summary>
        /// <returns>OperationResult to indicate whether the info got fetched from the database or not</returns>
        public static OperationResult GetQuestions()
        {
            try
            {
                mQuestionsList.Clear();
                return Database.GetQuestionsFromDB(ref mQuestionsList);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }
        }

        /// <summary>
        /// responsible for fetching the question general data
        /// </summary>
        /// <param name="pQuestionId">the Id of the quesiton whose information must be fetched</param>
        /// <returns>OperationResult to indicate whether the info got fetched from the database or not</returns>
        public static Question GetQuestionData(int pQuestionId)
        {
            try
            {
                Question tQuestionGeneralData = mQuestionsList.Find(question => question.Id == pQuestionId);
                return tQuestionGeneralData;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// get spcific question of the data based on its Id
        /// </summary>
        /// <param name="pQuestionId">the Id of the quesiton whose information must be fetched</param>
        /// <param name="pQuestionSpecificData">a reference question object to fill with the full question info</param>
        /// <returns>OperationResult to indicate whether the info got fetched from the database or not</returns>
        public static OperationResult GetQuestionSpecificData(int pQuestionId, ref Question pQuestionSpecificData)
        {
            try
            {
                //get the general question data to add to it its specific data
                Question tQuestionData = GetQuestionData(pQuestionId);
                OperationResult tQuestionSpecificDataResult = Database.GetQuestionSpecificDataFromDB(tQuestionData, ref pQuestionSpecificData);

                if (tQuestionSpecificDataResult.IsSuccess && pQuestionSpecificData == null)
                {
                    return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
                }
                else
                {
                    return tQuestionSpecificDataResult;
                }
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }
        }

        /// <summary>
        /// adds a new question object to the database, and the QuestionsList collection
        /// and notify the UI layer to update its data, note that the question won't
        /// be added to the QuesitonsList nor will the UI be notified if it isn't
        /// added to the database
        /// </summary>
        /// <param name="pQuestionData">full data of the question</param>
        /// <returns>OperationResult to indicate whether the question got added to the database or not</returns>
        public static OperationResult AddQuestion(Question pQuestionData)
        {
            try
            {
                //add the question to the database to generate its id and obtain it
                OperationResult tAddQuestionResult = Database.AddQuestionToDB(ref pQuestionData);
                //on successful question addition to Database add it to the Questions List
                if (tAddQuestionResult.IsSuccess)
                {
                    mQuestionsList.Add(pQuestionData);
                    //notify UI of change
                    DataBaseChangedEvent?.Invoke(typeof(QuestionOperations), EventArgs.Empty);
                }

                return tAddQuestionResult;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }
        }

        /// <summary>
        /// edit the question data, by getting its original type from the QuestionsList 
        /// collection and sending it along with the updated data to determine whether
        /// its type was changed or not, and as the previous one, the UI won't be 
        /// changed unless the database operation was successful.
        /// </summary>
        /// <param name="pUpdatedQuestionData">updated question data</param>
        /// <returns>OperationResult to indicate whether the question data go edited or not</returns>
        public static OperationResult UpdateQuestion(Question pUpdatedQuestionData)
        {
            try
            {
                eQuestionType tOriginalQuestionType = GetQuestionData(pUpdatedQuestionData.Id).Type;

                OperationResult tQuestionUpdatedResult = Database.UpdateQuestionOnDB(tOriginalQuestionType, pUpdatedQuestionData);
                if (tQuestionUpdatedResult.IsSuccess)
                {
                    //remove from questions list
                    mQuestionsList.Remove(mQuestionsList.Find(question => question.Id == pUpdatedQuestionData.Id));
                    //add the new Question to the list
                    mQuestionsList.Add(pUpdatedQuestionData);
                    //notify UI of change
                    DataBaseChangedEvent?.Invoke(typeof(QuestionOperations), EventArgs.Empty);
                }
                return tQuestionUpdatedResult;

            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }
        }

        /// <summary>
        /// delete the selected question/questions from db, notify UI of deletion
        /// only if it was successfull.
        /// </summary>
        /// <param name="pSelectedQuestions"></param>
        /// <returns>OperationResult to indicate whether the question data go deleted or not</returns>
        public static OperationResult DeleteQuestion(List<Question> pSelectedQuestions)
        {
            try
            {
                OperationResult tDeleteQuestionsResult = Database.DeleteQuestionFromDB(pSelectedQuestions);

                if (!tDeleteQuestionsResult.IsSuccess)
                {
                    return tDeleteQuestionsResult;
                }
                //delete question from List (Questions)
                foreach (Question tQuestion in pSelectedQuestions)
                {
                    mQuestionsList.Remove(tQuestion);
                }
                //notify UI of change
                DataBaseChangedEvent?.Invoke(typeof(QuestionOperations), EventArgs.Empty);
                return new OperationResult();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }
        }
        #endregion

        #region class utilty functions
        /// <summary>
        /// this function tests whether a connection can be established to the database
        /// </summary>
        /// <returns>OperationResult object to indicate the success or failure of the connection to database</returns>
        public static OperationResult TestDBConnection()
        {
            try
            {
                return Database.TestDataBaseConnection();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return new OperationResult(GlobalStrings.UnknownErrorTitle, GlobalStrings.UnknownError);
            }

        }

        /// <summary>
        /// this function obtains the connection string from the connectionString.json file
        /// if it exists, otherwise create the file if it doesn't exist.
        /// </summary>
        public static bool GetConnectionString()
        {
            //try to obtain the connection string from a file
            try
            {
                //check that file exists
                if (!File.Exists(mFilePath))
                {
                    //create json file and fill it with default stuff
                    using (FileStream tFs = File.Create(mFilePath)) ;
                    return false;
                }

                //check file content
                if (new FileInfo(mFilePath).Length == 0)
                {
                    return false;
                }

                //assuming that the file exists and it contains a connection string
                using (StreamReader tFileReader = new StreamReader(mFilePath))
                {
                    string tReadConnectionString = tFileReader.ReadToEnd();
                    //de-serialize the obtained connection string and transform it to the correct format
                    ConnectionString tDesrializedConnString = JsonSerializer.Deserialize<ConnectionString>(tReadConnectionString);
                    Database.mConnectionString = tDesrializedConnString.GetFormattedConnectionString();
                }
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                UtilityMethods.LogError(ex);
                return false;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// writes the connection string details to the ConnectionString.json file
        /// </summary>
        /// <param name="pConnectionString"></param>
        public static void SetConnectionString(ConnectionString pConnectionString)
        {
            try
            {
                using (StreamWriter tWriter = new StreamWriter(mFilePath))
                {
                    string tSerializedConnectionString = JsonSerializer.Serialize<ConnectionString>(pConnectionString);
                    tWriter.Write(tSerializedConnectionString);
                }
                Database.mConnectionString = pConnectionString.GetFormattedConnectionString();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }

        /// <summary>
        /// this function create a thread and starts a funciton on it to start monitoring the database
        /// the thread runs in the background as to not block the main thread it the function on the 
        /// thread.
        /// </summary>
        public static void StartCheckingDataBaseChange()
        {
            try
            {
                Thread tCheckThread = new Thread(() => CheckDataBaseChange(Thread.CurrentThread));
                tCheckThread.IsBackground = true;
                tCheckThread.Start();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }

        /// <summary>
        /// this function keeps checking the database every 10 seconds
        /// for any change, it uses the database checksum as the base
        /// for comparison, as it changes and produce a unique value 
        /// on every change, also the database won't be checked if 
        /// there's any operation going on (add, edit, delete) as to 
        /// avoid any errors or conflits in changing data.
        /// in the case of the database refusing to connect multiple times
        /// this function raises an event and returns a failed operationResult object.
        /// </summary>
        /// <param name="pMainThread"></param>
        public static void CheckDataBaseChange(Thread pMainThread)
        {
            try
            {
                int tDatabaseConnectionRetryCount = 0;
                //get checksum of the database current version of data
                long tcurrentChecksum = 0;
                Database.GetChecksum(ref tcurrentChecksum);
                //keep the function running while main thread is running
                while (pMainThread.IsAlive)
                {
                    Thread.Sleep(10000);
                    if (!mOperationOngoing)
                    {
                        //get checksum again to detect change
                        long tNewChecksum = 0;
                        OperationResult tNewChecksumResult = Database.GetChecksum(ref tNewChecksum);
                        if (tNewChecksumResult.IsSuccess)
                        {
                            if (tcurrentChecksum != tNewChecksum)
                            {
                                //data changed
                                tcurrentChecksum = tNewChecksum;

                                mQuestionsList.Clear();
                                Database.GetQuestionsFromDB(ref mQuestionsList);
                                //notify UI of database change
                                DataBaseChangedEvent?.Invoke(typeof(QuestionOperations), EventArgs.Empty);

                                //reset the connection retry counter on successful data change
                                tDatabaseConnectionRetryCount = 0;
                            }
                        }
                        else if (tNewChecksumResult.mErrorMessage == "Database was just created")
                        {
                            continue;
                        }
                        else
                        {
                            tDatabaseConnectionRetryCount++;
                            if (tDatabaseConnectionRetryCount > cDatabaseReconnectMaxAttempts)
                            {
                                DataBaseNotConnectedEvent?.Invoke(typeof(QuestionOperations), EventArgs.Empty);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }

        #endregion
    }
}
