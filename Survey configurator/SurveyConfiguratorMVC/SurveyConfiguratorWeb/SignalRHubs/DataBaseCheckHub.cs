using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using QuestionServices;

namespace SurveyConfiguratorWeb.SignalRHubs
{
    [HubName("DataBaseCheckHub")]
    public class DataBaseCheckHub : Hub
    {
        public DataBaseCheckHub() {

            if (!Startup.mIsDatabaseChangeEventRegistered) { 
                QuestionOperations.DataBaseChangedEvent += QuestionOperations_DataBaseChangedEvent;
                Startup.mIsDatabaseChangeEventRegistered = true;
            }
        }

        private  void QuestionOperations_DataBaseChangedEvent(object sender, System.EventArgs e)
        {
            //tell all clients to re-render their index page
            Clients.All.UpdateQuestionsList();
        }

        [HubMethodName("StartCheck")]
        public void StartCheck()
        {
            QuestionOperations.StartCheckingDataBaseConnection();
            QuestionOperations.StartCheckingDataBaseChange();
        }

    }
}