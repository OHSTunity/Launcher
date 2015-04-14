using System;
using System.Text;
using Polyjuice;
using Starcounter;

namespace Launcher {
    public class Program {
        public static void Main() {
            MainHandlers handlers = new MainHandlers();

            handlers.Register();
            LauncherHelper.Init();
        }
    }
}
