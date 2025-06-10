using Godot;

public partial class ExitMenuScene : Node
{
    // Access to autoloaded GDScript singleton
    private Node globals;

    [Signal]
    public delegate void ResumeFromQuitPromptEventHandler();

    public override void _Ready()
    {
        // Get reference to autoloaded GDScript singleton
        globals = GetNode<Node>("/root/Globals");
    }

    public override void _Process(double delta)
    {
    }

    private void DebugPrint(string msg)
    {
        // Access DEBUG_MODE from the Globals GDScript autoload
        bool debugMode = globals.Get("DEBUG_MODE").AsBool();
        if (debugMode)
        {
            GD.Print(msg);
        }
    }

    private void _on_cancel_btn_pressed()
    {
        DebugPrint("Cancel Button pressed");
        EmitSignal(SignalName.ResumeFromQuitPrompt);
    }
    
    private void _on_exit_btn_pressed()
    {
        DebugPrint("Quit the game");
        GetTree().Quit();
    }
}