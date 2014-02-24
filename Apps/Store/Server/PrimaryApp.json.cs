using Starcounter;

[PrimaryApp_json]
partial class PrimaryApp : Page {
    /// JSON objects can be manipulated on the server at any time. In this method, we add some items to a JSON array
    /// containing menu choices. These will be rendered on the client in the main side bar menu.
    public void AddSomeNiceMenuItems(Master master) {
        master.Menu.Items.Clear();                  // These are just JSON properties we created in the Master.json file. They can have any name and value.
        var create = master.Menu.Items.Add();       // Adds a new item to an array. The { Menu: Items: [ { ... } ] } can be found in Master.json.
        create.Label = "Compose new email";         // Let's have a menu button to allow us to create new emails.
        create.Uri = "/primary/create";             // This is the link to the page allowing the user to compose new emails
    }
}
