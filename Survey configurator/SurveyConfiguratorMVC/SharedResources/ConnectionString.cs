using System;
using System.Net.PeerToPeer;

namespace SharedResources
{
    public class ConnectionString
    {
        private const int cMaxTimeOutDuration = 5;

        /// <summary>
        /// a class to facilitate the process of obtaining the connection string
        /// and changing/saving it in the connectionString.json file
        /// </summary>
        public string mServer { get; set; }
        public string mDatabase { get; set; }
        public string mUser { get; set; }
        public string mPassword { get; set; }
        public bool mEncrypt { get; set; }
        public int mTimeout { get; set; }
        public bool mIntegratedSecurity { get; set; }

        public ConnectionString()
        {
            try
            {
                mServer = string.Empty;
                mDatabase = string.Empty;
                mIntegratedSecurity = true;
                mEncrypt = false;
                mTimeout = cMaxTimeOutDuration;
                mUser = string.Empty;
                mPassword = string.Empty;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }

        public ConnectionString(string pServer, string pDatabase, bool pIntegratedSecurity, string pUser= "", string pPassword = "")
        {
            try
            {
                mServer = pServer;
                mDatabase = pDatabase;
                mIntegratedSecurity = pIntegratedSecurity;
                mEncrypt = false;
                mTimeout = cMaxTimeOutDuration;
                mUser = pUser;
                mPassword = pPassword;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }

        /// <summary>
        /// return a connection string in its correct format
        /// </summary>
        /// <returns>correctly formatted connection string</returns>
        public string GetFormattedConnectionString()
        {
            try
            {
                return $"Server={mServer}; Database={mDatabase}; Integrated Security={mIntegratedSecurity}; User={mUser}; Password={mPassword}; Encrypt={mEncrypt}; Timeout={mTimeout}";
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return string.Empty;
            }
        }
    }
}
