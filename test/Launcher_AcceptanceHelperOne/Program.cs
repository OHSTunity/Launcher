using System;
using Starcounter;

namespace Launcher.AcceptanceHelperOne {
    class Program {
        static void Main() {
            Handle.GET("/Launcher_AcceptanceHelperOne", () => {
                var page = new Page() {
                    Html = "/Launcher_AcceptanceHelperOne/Master.html"
                };
                return page;
            });

            Handle.GET("/Launcher_AcceptanceHelperOne/menu", () => {
                var page = new Page() {
                    Html = "/Launcher_AcceptanceHelperOne/Menu.html"
                };
                return page;
            });

            UriMapping.Map("/Launcher_AcceptanceHelperOne/menu", UriMapping.MappingUriPrefix + "/menu");
        }
    }
}