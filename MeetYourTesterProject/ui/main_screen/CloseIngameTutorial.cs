using Godot;
using System;

public partial class CloseIngameTutorial : Node2D
{
    [Signal]
    public delegate void ResumeGameEventHandler();

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    private void OnQuitTutorialPressed()
    {
        this.Visible = false;
        EmitSignal(nameof(ResumeGame));
    }
}
