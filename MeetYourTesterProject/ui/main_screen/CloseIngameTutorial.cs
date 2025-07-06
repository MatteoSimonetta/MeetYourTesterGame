using Godot;

public partial class CloseIngameTutorial : Node
{
	[Signal]
	public delegate void ResumeGameEventHandler();

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	private void _on_quit_tutorial_pressed()
	{
		Set("visible", false);
		EmitSignal(SignalName.ResumeGame);
	}
}
