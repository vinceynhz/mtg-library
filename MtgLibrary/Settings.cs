using System.Configuration;
using System.Threading.Tasks;

namespace MtgLibrary
{
    class Settings 
    {
        public static Task<string> AsyncGetSetting(string key)
        {
            return Task.Run<string>(() => ConfigurationManager.AppSettings[key]);
        }
        
        public static Task<string> AsyncAddSetting(string key, string value)
        {
            return Task.Run<string>(() => Settings.Add(key, value));
        }
        public static string Add(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;
            settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
            return value;
        }

    }
}