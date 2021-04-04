using System.Configuration;
using System.Threading.Tasks;

namespace MtgLibrary
{
    class Settings
    {
        public const string API_MTG_URL_PROPERTY = "api.mtg.url";
        public const string API_MTG_CARD_PATH_PROPERTY = "api.mtg.card.path";
        public const string DB_LOCATION_PROPERTY = "db.location";
        public const string DB_NAME_PROPERTY = "db.name";
        public const string LOGGING_LEVEL = "logging.level";
        public static Task<string> AsyncGet(string key)
        {
            return Task.Run<string>(() => Settings.Get(key));
        }

        public static Task<string[]> AsyncGetAllKeys()
        {
            return Task.Run<string[]>(() => Settings.GetAllKeys());
        }

        public static Task<string> AsyncAdd(string key, string value)
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

        public static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string[] GetAllKeys()
        {
            return ConfigurationManager.AppSettings.AllKeys;
        }
    }
}