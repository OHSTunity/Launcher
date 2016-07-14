using Launcher.Helper;
using Starcounter;

namespace Launcher {
    partial class SettingsPage : Page, IBound<LauncherSettings>
    {
        void Handle(Input.Save action)
        {
            Transaction.Commit();
        }
    }
}
