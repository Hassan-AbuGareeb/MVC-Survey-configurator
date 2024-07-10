using SharedResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace SurveyConfiguratorWeb.ConstantsAndMethods
{
    public class SharedMethods
    {
        /// <summary>
        /// this class provides methods
        /// used across the different classes
        /// in the UI layer only
        /// </summary>

        /// <summary>
        /// this methods gets the current app language stored
        /// in the LanguageSettings xml file
        /// </summary>
        /// <returns>a string representing the current app language</returns>
        public static string GetAppLanguage()
        {
            try 
            { 
                //read the xml file
                XmlDocument tSettingsDocument = new XmlDocument();
                tSettingsDocument.Load(SharedConstants.cLanguageSettingsFilePath);

                //get the app language node
                XmlNode tLanguageNode = tSettingsDocument.SelectSingleNode(SharedConstants.cAppLanguageNode);

                var tCurrentAppLanguage = tLanguageNode.Attributes[SharedConstants.cAppLanguageSettingsValue].Value;
                if(string.IsNullOrEmpty(tCurrentAppLanguage))
                {
                    //return default value
                    return SharedConstants.cEnglishAppSettingsKey;
                }

                return tCurrentAppLanguage;
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                //return the default language
                return SharedConstants.cEnglishAppSettingsKey;
            }
        }

        public static void SetAppLanguage(string pNewLanguage)
        {
            try
            {
                //read the xml file
                XmlDocument tSettingsDocument = new XmlDocument();
                tSettingsDocument.Load(SharedConstants.cLanguageSettingsFilePath);

                //get the app language node
                XmlNode tLanguageNode = tSettingsDocument.SelectSingleNode(SharedConstants.cAppLanguageNode);

                // set the app language element value 
                tLanguageNode.Attributes[SharedConstants.cAppLanguageSettingsValue].Value = pNewLanguage;

                //save changes
                tSettingsDocument.Save(SharedConstants.cLanguageSettingsFilePath);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }
    }
}