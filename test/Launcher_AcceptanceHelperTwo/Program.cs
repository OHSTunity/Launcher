using System;
using Launcher_AcceptanceHelperTwo;
using Starcounter;

namespace Launcher.AcceptanceHelperTwo {
    class Program {
        static void Main() {
            Handle.GET("/Launcher_AcceptanceHelperTwo", () => Db.Scope(() => new Launcher_AcceptanceHelperTwoPage().Init()));

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