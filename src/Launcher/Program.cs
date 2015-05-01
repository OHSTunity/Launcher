using System;
using System.Text;
using Polyjuice;
using Starcounter;

namespace LauncherNamespace {
    public class Program {
        public static void Main() {
            MainHandlers handlers = new MainHandlers();

            handlers.Register();
            LauncherHelper.Init();
        }
    }
}
