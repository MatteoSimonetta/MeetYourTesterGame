using Godot;
using System;

public partial class ExitMenuScene : Node2D
{
    [Signal]
    public delegate void ResumeFromQuitPromptEventHandler();

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    private void DebugPrint(string msg)
    {
        if (Globals.DEBUG_MODE)
        {
            GD.Print(msg);
        }
    }

    private void OnCancelBtnPressed()
    {
        DebugPrint("Cancel Button pressed");
        EmitSignal(SignalName.ResumeFromQuitPrompt);
    }

    private void OnExitBtnPressed()
    {
        DebugPrint("Quit the game");
        GetTree().Quit();
    }
}
