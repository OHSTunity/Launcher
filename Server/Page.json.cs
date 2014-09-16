using Starcounter;

[Page_json]
partial class Page : Json {

    // Browsers will ask for "text/html" and we will give it to them
    // by loading the contents of the URI in our Html property
    public override string AsMimeType(MimeType type) {
        if (type == MimeType.Text_Html) {
            return X.GET<string>(Html);
        }
        return base.AsMimeType(type);
    }

    /// <summary>
    /// The way to get a URL for HTML partial if any.
    /// </summary>
    /// <returns></returns>
    public override string GetHtmlPartialUrl() {
        return Html;
    }

    /// <summary>
    /// Whenever we set a bound data object to this page, we update the
    /// URI property on this page.
    /// </summary>
    protected override void OnData() {
        base.OnData();
        var str = "";
        Json x = this;
        while (x != null) {
            if (x is Page)
                str = (x as Page).UriFragment + str;
            x = x.Parent;
        }
        Uri = str;
    }

    /// <summary>
    /// Override to provide an URI fragment
    /// </summary>
    /// <returns></returns>
    protected virtual string UriFragment {
        get {
            return "";
        }
    }
}
