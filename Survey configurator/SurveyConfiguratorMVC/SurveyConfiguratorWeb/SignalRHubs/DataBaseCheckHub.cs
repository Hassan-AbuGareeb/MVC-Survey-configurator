using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using QuestionServices;
using SharedResources;
using System;

namespace SurveyConfiguratorWeb.SignalRHubs
{
    [HubName("DataBaseCheckHub")]
    public class DataBaseCheckHub : Hub
    {
        /// <summary>
        /// SignalR hub which provides services for the subscribing
        /// clients, currently it provides database related services
        /// such as notifying clients when the database gets updated, 
        /// and monitoring database connection status
        /// </summary>

        /// <summary>
        /// the constructor for this hub create handlers for the events
        /// such as the databasechange event, and makes sure that the event
        /// is registered only once.
        /// </summary>
        public DataBaseCheckHub() {
            try 
            { 
                if (!Startup.mIsDatabaseChangeEventRegistered) { 
                    QuestionOperations.DataBaseChangedEvent += QuestionOperations_DataBaseChangedEvent;
                    Startup.mIsDatabaseChangeEventRegistered = true;
                }
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }

        /// <summary>
        /// event handler for the database change event, notifies all clients
        /// of update and call the appropriate method on all clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void QuestionOperations_DataBaseChangedEvent(object sender, System.EventArgs e)
        {
            try 
            { 
                //notify all clients of change to re-render their index page
                Clients.All.UpdateQuestionsList();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }

        /// <summary>
        /// this method starts monitoring the database for change
        /// and maintining the database connection state
        /// </summary>
        [HubMethodName("StartCheck")]
        public void StartCheck()
        {
            try 
            { 
                QuestionOperations.StartCheckingDataBaseConnection();
                QuestionOperations.StartCheckingDataBaseChange();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }

    }
}