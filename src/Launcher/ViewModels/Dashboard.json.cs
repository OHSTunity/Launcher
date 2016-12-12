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
using Concepts.Ring8.Tunity;
using Colab.Common;

namespace Launcher
{

    [Dashboard_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
    partial class Dashboard : Page, IBound<TunityUser>
    {
       
    }




}