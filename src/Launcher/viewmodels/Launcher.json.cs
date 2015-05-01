using Starcounter;                                  // Most stuff relating to the database, JSON and communication is in this namespace
using Starcounter.Templates;
using Starcounter.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using Starcounter.Advanced.XSON;
using PolyjuiceNamespace;

namespace LauncherNamespace {

    [Launcher_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
    partial class Launcher : Page {
    }

    [Launcher_json.searchBar]
    partial class SearchBar : Json {
        void Handle(Input.query query) {

            string uri = "/launcher/search?query=" + HttpUtility.UrlEncode(query.Value);

            Response resp = X.GET(uri);
            searchEngineResultPageUrl = uri;
            if (resp != null) {
                this.Parent = resp;
            }
        }
    }

}