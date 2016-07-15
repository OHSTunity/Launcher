using System;
using System.Text.RegularExpressions;
using Starcounter;
using Launcher.Database;

namespace Launcher.Helper
{

    public static class SettingsHelper
    {
        public static LauncherSettings GetSettings()
        {
            var settings = Db.SQL<LauncherSettings>("SELECT ls FROM LauncherSettings ls").First;
            if(settings == null)
            {
                Db.Transact(() =>
                {
                    settings = new LauncherSettings();
                });
            }
            return settings;
        }

        public static bool IfBaypassUrl(string uri)
        {
            var urlPattern = GetSettings().IgnoreUrlPattern;
            if (string.IsNullOrEmpty(urlPattern)) return false;

            try
            {
                return Regex.IsMatch(urlPattern, uri);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
