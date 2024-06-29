using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using QuestionServices;

namespace SurveyConfiguratorWeb.SignalRHubs
{
    [HubName("DataBaseCheckHub")]
    public class DataBaseCheckHub : Hub
    {
        //a function is needed here 
        public DataBaseCheckHub() {

            if (!Startup.mIsEventRegistered) { 
                QuestionOperations.DataBaseChangedEvent += QuestionOperations_DataBaseChangedEvent;
                Startup.mIsEventRegistered = true;
            }
        }

        private async void QuestionOperations_DataBaseChangedEvent(object sender, System.EventArgs e)
        {
            //tell all clients to re-render their index page
            await Clients.All.UpdateQuestionsList();
        }

        [HubMethodName("StartCheck")]
        public void StartCheck()
        {
            QuestionOperations.StartCheckingDataBaseChange();
        }

    }
}