using Godot;
using System;

public partial class ActionEventCommunication : Node
{
    public void SendToTerminal(string args)
    {
        GD.Print("sending data");

        var siblingB = GetParent().GetNodeOrNull<Node>("MainGameScene");
        if (siblingB != null)
        {
            GD.Print("Got the node " + siblingB.Name);
            siblingB.Call("HandleEventFromActionEvent", args);
        }
    }
}
