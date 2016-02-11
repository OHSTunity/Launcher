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

namespace Launcher {

    [LauncherPage_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
    partial class LauncherPage : Page {
        [LauncherPage_json.searchBar]
        partial class SearchBar : Json
        {
            void Handle(Input.query action)
            {
                string uri = UriMapping.MappingUriPrefix + "/search?query=" + HttpUtility.UrlEncode(action.Value);
                this.previewVisible = true;
                this.previewResult = Self.GET<Json>(uri);
            }

            void Handle(Input.submit action)
            {
                string uri = "/launcher/search?query=" + this.query;
                Response resp = Self.GET(uri);

                searchEngineResultPageUrl = uri;
            }

            void Handle(Input.close action)
            {
                this.previewResult = null;
                this.previewVisible = false;
            }
        }
    }

    

 
}