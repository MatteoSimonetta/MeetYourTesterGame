using Godot;
using System;
using Godot.Collections;

public partial class TerminalCommunication : Node2D
{
    [Export]
    public string EventName = "";

    public override void _Ready()
    {
        // List of hexagon nodes
        var hexList = new Node[]
        {
            GetNode("../../MainControl/Database"),
            GetNode("../../MainControl/Delivery"),
            GetNode("../../MainControl/Business_Logic"),
            GetNode("../../MainControl/Backend"),
            GetNode("../../MainControl/UI_UX")
        };

        // Read and parse all questions from different JSON files
        foreach (string node in new[] { "Database", "Delivery", "Business_Logic", "Backend", "UI_UX", "GutTest" })
        {
            string path = $"{Globals.Instance.QuestionsDirPath}/{node}.json";
            if (FileAccess.FileExists(path))
            {
                string file = FileAccess.GetFileAsString(path);
                var parsed = Json.ParseString(file);
                Globals.Instance.Questions[node] = parsed;
            }
            else
            {
                Globals.Instance.Questions[node] = new Godot.Collections.Array();
            }
        }

        // Connect each hexagon signal
        foreach (var hex in hexList)
        {
            hex.Connect("HexagonClicked", new Callable(this, nameof(HandleEventFromActionEvent)));
        }
    }

    private void HandleEventFromActionEvent(Dictionary eventParams)
    {
        EventName = eventParams["node_name"].AsString();

        var eventQuestions = Globals.Instance.Questions[EventName];
        var terminalContent = GetNode("terminal_content");

        terminalContent.Call("handle_event_from_action_event", EventName, eventQuestions);
    }
}
