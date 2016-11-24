using Starcounter;
using System;
using System.Web;

namespace Launcher {
    partial class AdvancedSearchPage : Page
    {
        void Handle(Input.Query input)
        {
            if (String.IsNullOrEmpty(input.Value))
            {
                this.Result = null;
                return;
            }
            string uri = UriMapping.MappingUriPrefix + "/search?query=" + HttpUtility.UrlEncode(input.Value) + 
                "&count=" + Count.ToString() + "&start=" + StartAt.ToString();
            this.Result = Self.GET<Json>(uri, () =>
                 {
                     var p = new Page();
                     return p;
                 });
        }

        public String OptionsCB
        {
            get
            {
                return OptionsCB;
            }
            set
            {
                
            }
        }
    }
}
