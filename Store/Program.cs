using System;
using Starcounter;
using Starcounter.Internal;
using Starcounter.Templates;

namespace Store
{
    class Program
    {
        static void Main()
        {
            StarcounterEnvironment.AppName = "App3";
            Handle.GET("/person/{?}", (String personId) =>
            {
                var sTemplate = new TObject();
                sTemplate.Add<TString>("storeName$");
                sTemplate.Add<TTrigger>("buy$");
                sTemplate.Add<TString>("Html");

                dynamic s = new Json();
                s.Template = sTemplate;
                s.storeName = "";
                s.Html = "<article><input value=\"{{storeName$}}\"><button onclick=\"this.model.call$ = null\" value=\"{{buy$}}\">call</button></article>";
                return s;
            });
        }
    }
}