using Godot;

public partial class TerminalCommunication : Node2D
{
    [Export]
    public string EventName { get; set; } = "";

    // Access to autoloaded GDScript singleton
    private Node globals;

    public override void _Ready()
    {
        // Get reference to autoloaded GDScript singleton
        globals = GetNode<Node>("/root/Globals");
        
        // List of all hexagons
        var hexList = new Node[]
        {
            GetNode("../../MainControl/Database"),
            GetNode("../../MainControl/Delivery"),
            GetNode("../../MainControl/Business_Logic"),
            GetNode("../../MainControl/Backend"),
            GetNode("../../MainControl/UI_UX")
        };

        // Read and parse all questions from different data files for each node (in JSON)
        var nodeNames = new string[] { "Database", "Delivery", "Business_Logic", "Backend", "UI_UX", "GutTest" };
        var questionsDict = new Godot.Collections.Dictionary();
        
        foreach (string nodeName in nodeNames)
        {
            string questionsDirPath = globals.Get("questions_dir_path").AsString();
            string filePath = $"{questionsDirPath}/{nodeName}.json";
            string file = FileAccess.GetFileAsString(filePath);
            
            if (!string.IsNullOrEmpty(file))
            {
                var json = new Json();
                json.Parse(file);
                questionsDict[nodeName] = json.Data;
            }
            else
            {
                questionsDict[nodeName] = new Godot.Collections.Array();
            }
        }
        
        // Set the questions dictionary in globals
        globals.Set("questions", questionsDict);

        // Connect terminal to each hexagon in game scene
        foreach (Node hex in hexList)
        {
            hex.Connect("hexagon_clicked", new Callable(this, nameof(HandleEventFromActionEvent)));
        }
    }

    private void HandleEventFromActionEvent(Godot.Collections.Dictionary eventParams)
    {
        EventName = eventParams["node_name"].AsString();
        
        // Retrieve stored questions for this action event
        var questionsDict = globals.Get("questions").AsGodotDictionary();
        var eventQuestions = questionsDict[EventName];
        
        // Send signal to terminal to show the questions
        var terminalContent = GetNode("terminal_content");
        terminalContent.Call("handle_event_from_action_event", EventName, eventQuestions);
    }
}