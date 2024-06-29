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

            if (!QuestionOperations.mIsEventRegistered) { 
                QuestionOperations.DataBaseChangedEvent += QuestionOperations_DataBaseChangedEvent;
                QuestionOperations.mIsEventRegistered = true;
            }
        }

        private async void QuestionOperations_DataBaseChangedEvent(object sender, System.EventArgs e)
        {
            await Clients.All.sayHello("All changed yo yo");
        }

        [HubMethodName("StartCheck")]
        public void StartCheck()
        {
            QuestionOperations.StartCheckingDataBaseChange();
        }

        public void Hello(string message)
        {
            Clients.All.sayHello(message);
        }
    }
}