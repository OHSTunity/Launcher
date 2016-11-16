using System;
using System.Text;
using Launcher.Helper;
using Starcounter;

namespace Launcher {
    public class Program {
        public static void Main() {
            MainHandlers handlers = new MainHandlers();

            handlers.Register();
            LauncherHelper.Init();

            Http.GET(8080, "http://sc/alias/8080/;/launcher/launchpad");
            Http.GET(8080, "http://sc/alias/8080/mobile;/launcher/mobile");
            Http.GET(8080, "http://sc/alias/8080/mobile/dashboard;/launcher/mobile/dashboard");
        }
    }
}
