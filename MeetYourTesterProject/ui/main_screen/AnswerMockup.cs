using Godot;

public partial class AnswerMockup : Button
{
    public override void _Ready()
    {
        // Connect the pressed signal
        Pressed += OnPressed;
    }

    public override void _Process(double delta)
    {
    }
    
    private void OnPressed()
    {
        var terminalMockNode = GetParent().GetNode("_terminal_mock");
        string nodeName = terminalMockNode.Get("event_name").AsString();
        var node = GetNode("../../MainControl/" + nodeName);
        
        GD.Print("mockup answer selected for node " + nodeName);
        
        node.Call("remove_action_event");
        
        Visible = false;
        Disabled = true;
    }
}