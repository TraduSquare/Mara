using System;
using System.IO;

namespace Mara.Lib.Common.Steam.Api
{
     public class SteamLocalConfig {
        private readonly string steam_location;
        private readonly string steam_userdata_location;
        private bool foundSteamLocation;

        public SteamLocalConfig() {
            steam_userdata_location = SteamUtils.GetUserDataFolderPath();
        }

        /// <summary>
        /// Returns a setting of the first found app setting from all steam users
        /// </summary>
        /// <param name="_appId"></param>
        /// <param name="_settingKey"></param>
        /// <param name="_settingValue"></param>
        /// <returns>Setting value</returns>
        public bool GetFirstFoundAppSetting(string _appId, string _settingKey, out string _settingValue) {
            string[] dir = Directory.GetDirectories(steam_userdata_location);
            string fileName;
            bool valid;

            //Any directories found?
            for (int i = 0; i < dir.Length; i++) { 
                //Get the data from the first found setting from any user
                fileName = dir[i].Remove(0, steam_userdata_location.Length + 1);
                valid = GetAppSetting(fileName, _appId, _settingKey, out _settingValue);
                if (valid) return true;
            }

            _settingValue = null;
            return false;
        }

        private string CreateLocalConfigPath(string _userId) {
            string file = Path.Combine(steam_userdata_location, _userId);
            file = Path.Combine(file, @"config\localconfig.vdf");
            return file;
        }

        private bool GetAppSetting(string _userId, string _appId, string _settingKey, out string _settingValue) {
            _settingValue = "";

            string file = CreateLocalConfigPath(_userId);

            //Read the whole config data
            string configData;
            using (StreamReader sr = new StreamReader(file)) {
                configData = sr.ReadToEnd();
            }

            //Did we pull any data?
            if (configData != null && configData.Length > 0) {
                string key = string.Concat(_settingKey, "\"\t\t\"");
                //Find where the settings for this app starts within the config file
                int startIndex = configData.IndexOf(string.Concat('"', _appId, '"', "\n\t\t\t\t\t"));
                //The App ID does not exist (The game need to run atleast once)
                if (startIndex == -1) return false;
                //Find the end of this apps settings
                int endIndex = configData.IndexOf('}', startIndex);

                //Find the Launch Options
                int launchOptionsIndex = configData.IndexOf(key, startIndex);

                //There are no existing launch options
                if (launchOptionsIndex >= 0 || launchOptionsIndex < endIndex) {
                    int lineEnd = configData.IndexOf("\"\n\t\t\t\t\t", launchOptionsIndex);

                    //Get the setting value
                    if (lineEnd - launchOptionsIndex - key.Length > 0) {
                        _settingValue = configData.Substring(launchOptionsIndex + key.Length, lineEnd - launchOptionsIndex - key.Length);
                    }
                }
                return true;
            }

            _settingValue = null;
            return false;
        }

        /// <summary>
        /// Sets a setting for an AppID for all steam users
        /// </summary>
        /// <param name="_appId"></param>
        /// <param name="_settingKey"></param>
        /// <param name="_settingValue"></param>
        /// <returns></returns>
        public bool SetAppSetting(string _appId, string _settingKey, string _settingValue) {
            string[] dir = Directory.GetDirectories(steam_userdata_location);

            //Assume success if we have any user ID's to update
            bool success = dir.Length > 0;

            //Update the config for each user found
            string fileName;
            for (int i = 0; i < dir.Length; i++) {
                fileName = dir[i].Remove(0, steam_userdata_location.Length + 1);
                //Set false if at least one fails
                if (!SetAppSetting(fileName, _appId, _settingKey, _settingValue)) success = false;
            }

            if (!success)
            {
                Console.WriteLine("aaaaaaaaaaaa");
            }

            return success;
        }

        private bool SetAppSetting(string _userId, string _appId, string _settingKey, string _settingValue) {
            //1. Read the config file
            //2. Change the setting
            //3. Write back the config file

            string file = CreateLocalConfigPath(_userId);

            //Read the whole config data
            string configData;
            using (StreamReader sr = new StreamReader(file)) {
                configData = sr.ReadToEnd();
            }

            //Did we pull any data?
            if (configData != null && configData.Length > 0) {
                string key = string.Concat(_settingKey, "\"\t\t\"");
                //Find where the settings for this app starts within the config file
                int startIndex = configData.IndexOf(string.Concat('"', _appId, '"', "\n\t\t\t\t\t"));
                //The App ID does not exist (The game need to run atleast once)
                if (startIndex == -1) return false;
                //Get all the text from the start of this apps settings to the EOF
                //string temp = config_data.Substring(startIndex);
                //Find the end of this apps settings
                int endIndex = configData.IndexOf('}', startIndex);

                //Find the Launch Options
                //int launchOptionsIndex = configData.IndexOf("LaunchOptions", startIndex);
                int launchOptionsIndex = configData.IndexOf(key, startIndex);

                //There are no existing launch options
                if (launchOptionsIndex == -1 || launchOptionsIndex >= endIndex) {
                    // 1 Start at endIndex
                    // 2 Move backwards
                    // 3 Is this a tab (\t)?
                    // 3a YES -  Goto 2
                    // 3b NO - Insert new line
                    // 4 Tab in (counted from 2-3)
                    // 5 Insert launch options

                    int pos = endIndex;
                    int numOfTabs = 0;
                    bool done = false;
                    while (!done) {
                        pos -= 1;                       //Move backwards accounting for tabs (\t)
                        if (configData[pos] == '\t') numOfTabs++; //Count tabs found
                        else done = true;               //No more tabs found
                    }

                    //Insert line break for the setting
                    pos += 1;
                    
                    
                    // Check if the "cloud" key exists within the next 4 lines
                    bool isCloudKeyExists = false;
                    int lineCounter = 0;
                    int currentLineIndex = pos;
                    int cloudKeyStartIndex = configData.LastIndexOf(string.Concat('"', "cloud", '"', "\n\t\t\t\t\t", startIndex));
                    if (cloudKeyStartIndex != -1)
                        isCloudKeyExists = true;

                    // If the "cloud" key exists, insert the setting one line further
                    if (isCloudKeyExists)
                        pos = configData.IndexOf("\n", pos) + "\n".Length;
                    
                    configData = configData.Insert(pos, "\n");

                    //Insert the Launch Options, retaining the formatting of the file
                    configData = configData.Insert(pos, string.Concat(
                        new string('\t', numOfTabs + 1),
                        string.Concat('"', _settingKey, '"'), 
                        "\t\t",
                        string.Concat('"', _settingValue, '"'))
                    );
                    //Write the back the changes to the file
                    File.WriteAllText(file, configData);
                    return true;
                }
                //There are existing launch options
                else {
                    //Find the EOL for the Launch Options
                    int lineEnd = configData.IndexOf("\"\n\t\t\t\t\t", launchOptionsIndex);

                    //Remove the old setting value
                    if (lineEnd - launchOptionsIndex - key.Length > 0)
                        configData = configData.Remove(launchOptionsIndex + key.Length, lineEnd - launchOptionsIndex - key.Length);

                    //Add the new setting value
                    configData = configData.Insert(launchOptionsIndex + key.Length, _settingValue);
                    //Write the back the changes to the file
                    File.WriteAllText(file, configData);
                    return true;
                }
            }
            return false;
        }
    }
}
