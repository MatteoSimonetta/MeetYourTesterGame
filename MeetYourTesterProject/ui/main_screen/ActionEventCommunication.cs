using Godot;

public partial class ActionEventCommunication : Node
{
    private void SendToTerminal(string args)
    {
        GD.Print("sending data");
        string text = args;
        var siblingB = GetParent().GetNode("MainGameScene");
        
        if (siblingB != null)
        {
            GD.Print("Got the node " + siblingB.Name);
            siblingB.Call("handle_event_from_action_event", text);
        }
    }
}