using System;
using Starcounter;

namespace Launcher.AcceptanceHelperTwo {
    class Program {
        static void Main() {
            Handle.GET("/Launcher_AcceptanceHelperTwo", () => {
                var page = new Page() {
                    Html = "/Launcher_AcceptanceHelperTwo/Master.html"
                };
                return page;
            });

            Handle.GET("/Launcher_AcceptanceHelperTwo/menu", () => {
                var page = new Page() {
                    Html = "/Launcher_AcceptanceHelperTwo/Menu.html"
                };
                return page;
            });

            UriMapping.Map("/Launcher_AcceptanceHelperTwo/menu", UriMapping.MappingUriPrefix + "/menu");
        }
    }
}