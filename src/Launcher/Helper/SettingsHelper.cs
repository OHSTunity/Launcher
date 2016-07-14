using System;
using System.Text.RegularExpressions;
using Starcounter;

namespace Launcher.Helper
{

    public static class SettingsHelper
    {
        public static LauncherSettings GetSettings => Db.SQL<LauncherSettings>("SELECT ls FROM LauncherSettings ls").First;

        public static bool IfBaypassUrl(string uri)
        {
            var urlPattern = GetSettings?.IgnoreUrlPattern;
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
