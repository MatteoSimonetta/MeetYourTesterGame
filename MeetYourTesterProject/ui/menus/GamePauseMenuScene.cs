using Godot;

public partial class GamePauseMenuScene : Node2D
{
	private TextureButton startIcon;
	private TextureButton startLabel;
	private TextureButton quitIcon;
	private TextureButton quitLabel;
	private TextureButton tutorialIcon;
	private TextureButton tutorialLabel;
	
	// Access to autoloaded GDScript singleton
	private Node globals;

	[Signal]
	public delegate void ResumeGameEventHandler();
	
	[Signal]
	public delegate void OpenTutorialEventHandler();
	
	[Signal]
	public delegate void QuitEventHandler();

	public override void _Ready()
	{
		// Get reference to autoloaded GDScript singleton
		globals = GetNode<Node>("/root/Globals");
		
		// Get UI elements
		startIcon = GetNode<TextureButton>("GridContainer/StartIcon");
		startLabel = GetNode<TextureButton>("GridContainer/CenterStartLabel/StartLabel");
		quitIcon = GetNode<TextureButton>("GridContainer/QuitIcon");
		quitLabel = GetNode<TextureButton>("GridContainer/CenterQuitLabel/QuitLabel");
		tutorialIcon = GetNode<TextureButton>("GridContainer/TutorialIcon");
		tutorialLabel = GetNode<TextureButton>("GridContainer/CenterTutorialLabel/TutorialLabel");
	}

	public override void _Process(double delta)
	{
		// Empty - keeping for consistency with original
	}

	private void _on_resume_game_pressed()
	{
		DebugPrint("Resume game");
		EmitSignal(SignalName.ResumeGame);
	}

	private void _on_start_tutorial_pressed()
	{
		DebugPrint("Start tutorial");
		EmitSignal(SignalName.OpenTutorial);
	}

	private void _on_quit_pressed()
	{
		DebugPrint("Quit");
		EmitSignal(SignalName.Quit);
	}

	private void _on_start_label_mouse_entered()
	{
		DebugPrint("Start Game Button (label) on hover entered");
		startIcon.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-icon-return-select.svg");
	}

	private void _on_start_label_mouse_exited()
	{
		DebugPrint("Start Game Button (label) on hover exited");
		startIcon.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-icon-return.svg");
	}

	private void _on_start_icon_mouse_entered()
	{
		DebugPrint("Start Game Button (icon) on hover entered");
		startLabel.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-label-return-select.svg");
	}

	private void _on_start_icon_mouse_exited()
	{
		DebugPrint("Start Game Button (icon) on hover exited");
		startLabel.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-label-return.svg");
	}

	private void _on_quit_icon_mouse_entered()
	{
		DebugPrint("Quit Game Button (icon) on hover entered");
		quitLabel.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-label-quit-select.svg");
	}

	private void _on_quit_icon_mouse_exited()
	{
		DebugPrint("Quit Game Button (icon) on hover exited");
		quitLabel.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-label-quit.svg");
	}

	private void _on_quit_label_mouse_entered()
	{
		DebugPrint("Quit Game Button (label) on hover entered");
		quitIcon.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-icon-quit-select.svg");
	}

	private void _on_quit_label_mouse_exited()
	{
		DebugPrint("Quit Game Button (label) on hover exited");
		quitIcon.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-icon-quit.svg");
	}

	private void _on_tutorial_icon_mouse_entered()
	{
		DebugPrint("tutorial Game Button (icon) on hover entered");
		tutorialLabel.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-label-tutorial-select.svg");
	}

	private void _on_tutorial_icon_mouse_exited()
	{
		DebugPrint("tutorial Game Button (icon) on hover exited");
		tutorialLabel.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-label-tutorial.svg");
	}

	private void _on_tutorial_label_mouse_entered()
	{
		DebugPrint("tutorial Game Button (label) on hover entered");
		tutorialIcon.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-icon-tutorial-select.svg");
	}

	private void _on_tutorial_label_mouse_exited()
	{
		DebugPrint("tutorial Game Button (label) on hover exited");
		tutorialIcon.TextureNormal = GD.Load<Texture2D>("res://images/pause-menu/btn-icon-tutorial.svg");
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
}
