using System;
using System.Text;
using Launcher.Helper;
using System.Threading;
using Starcounter;

namespace Launcher {
    public class Program {
        public static void Main() {
            MainHandlers handlers = new MainHandlers();

            handlers.Register();
            LauncherHelper.Init();

            ThreadPool.QueueUserWorkItem(o => { 
                Http.GET(8181, "http://localhost/sc/alias/8080/;/launcher/launchpad");
                Http.GET(8181, "http://localhost/sc/alias/8080/settings;/launcher/settings");
                Http.GET(8181, "http://localhost/sc/alias/8080/mobile;/launcher/mobile");
                Http.GET(8181, "http://localhost/sc/alias/8080/mobile/dashboard;/launcher/mobile/dashboard");
            });
        }
    }
}
