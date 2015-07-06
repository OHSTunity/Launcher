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

namespace Launcher {

    [LauncherPage_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
    partial class LauncherPage : Page {
    }

    [LauncherPage_json.searchBar]
    partial class SearchBar : Json {
        void Handle(Input.query action) {
            string uri = "/polyjuice/search?query=" + HttpUtility.UrlEncode(action.Value);

            this.previewVisible = true;
            this.preview = Self.GET<SearchBarPreviewPage>(uri, () => {
                var p = new SearchBarPreviewPage();
                return p;
            });
        }

        void Handle(Input.submit action) {
            string uri = "/launcher/search?query=" + this.query;
            Response resp = Self.GET(uri);

            searchEngineResultPageUrl = uri;
        }

        void Handle(Input.close action) {
            this.preview = new SearchBarPreviewPage();
            this.previewVisible = false;
        }
    }

    [LauncherPage_json.searchBar.preview]
    partial class SearchBarPreviewPage : Page { 

    }

    [LauncherPage_json.results]
    public partial class LauncherResultsPage : Page {
    }
}