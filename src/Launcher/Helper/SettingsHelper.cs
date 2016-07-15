using System;
using System.Text.RegularExpressions;
using Starcounter;

namespace Launcher.Helper
{

    public static class SettingsHelper
    {
        public static LauncherSettings Settings => Db.SQL<LauncherSettings>("SELECT ls FROM LauncherSettings ls").First ?? new LauncherSettings();

        public static bool IfBaypassUrl(string uri)
        {
            var urlPattern = Settings.IgnoreUrlPattern;
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
