using Microsoft.Extensions.Configuration;
using RPFO.Utilities.Helpers;
using System.Configuration;
using System.IO;

namespace MWI.API.Helpers
{
    public class Utils
    {
        private static IConfiguration Configuration { get; set; }

        private static IConfigurationSection GetAppSettingsSection()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var exists = builder.GetFileProvider().GetFileInfo("appsettings.json").Exists;
            if (exists)
            {
                Configuration = builder.Build();
                var conn = Configuration.GetSection("AppSettings");
                return conn;
            }
            else
            {
                return null;
            }
        }

        public static string GetConfigByKey(string key)
        {
            string data = "";
            var appSetting = GetAppSettingsSection();
            if (appSetting != null)
            {
                data = Encryptor.DecryptString(appSetting[key], RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            }

            return data;
        }
    }
}
