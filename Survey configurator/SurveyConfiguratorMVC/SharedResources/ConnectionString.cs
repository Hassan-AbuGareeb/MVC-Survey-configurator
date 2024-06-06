using System;

namespace SharedResources
{
    public class ConnectionString
    {
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
                mUser = string.Empty;
                mPassword = string.Empty;
                mEncrypt = false;
                mTimeout = 5;
                mIntegratedSecurity = true;
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
