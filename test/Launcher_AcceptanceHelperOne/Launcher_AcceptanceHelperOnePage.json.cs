using System;
using System.Diagnostics;
using System.Linq;
using Simplified.Ring1;
using Starcounter;

namespace Launcher_AcceptanceHelperTwo
{
    partial class Launcher_AcceptanceHelperOnePage : Json
    {
        public Launcher_AcceptanceHelperOnePage Init()
        {
            var magicKey = "LauncherAcceptanceHelpers_SharedSomething";

            var somethings = Db.SQL<Something>("Select s from Simplified.Ring1.Something s where s.name = ?", magicKey);
            if (somethings.Any())
            {
                SharedValue.Data = somethings.First();
            }
            else
            {
                SharedValue.Data = new Something { Name = magicKey, Description = "Try changing this value" };
                Transaction.Commit();
            }
            SharedValue.AutoRefreshBoundProperties = false;

            return this;
        }

        [Launcher_AcceptanceHelperOnePage_json.SharedValue]
        public partial class SharedItem : Json, IBound<Something>
        {
            public void Handle(Input.Description action) {
                try {
                    Data.Description = action.Value;
                    Transaction.Commit();
                }
                catch (Exception e)
                {
                    Debugger.Launch();
                    Debugger.Break();
                }
            }
        }

    }
}
