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
            var port = Starcounter.Internal.StarcounterEnvironment.Default.UserHttpPort;
            ThreadPool.QueueUserWorkItem(o => {
                
                Http.GET(8181, String.Format("http://localhost/sc/alias/{0}/;/launcher/container_dashboard", port));
                Http.GET(8181, String.Format("http://localhost/sc/alias/{0}/dashboard;/launcher/container_dashboard", port));
                Http.GET(8181, String.Format("http://localhost/sc/alias/{0}/settings;/launcher/settings", port));
                Http.GET(8181, String.Format("http://localhost/sc/alias/{0}/configs;/launcher/configs", port));
                Http.GET(8181, String.Format("http://localhost/sc/alias/{0}/mobile;/launcher/mobile", port));
                Http.GET(8181, String.Format("http://localhost/sc/alias/{0}/mobile/dashboard;/launcher/mobile/dashboard", port));
            });
        }
    }
}
