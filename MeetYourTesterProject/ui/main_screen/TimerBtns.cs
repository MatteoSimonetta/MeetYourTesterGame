using Godot;

public partial class TimerBtns : Node
{
	[Signal]
	public delegate void PauseGameEventHandler();
	
	[Signal]
	public delegate void UnpauseGameEventHandler();
	
	[Signal]
	public delegate void SpeedUpGameEventHandler();

	private Texture2D playTexturePath = GD.Load<Texture2D>("res://images/game-map/time/play.svg");
	private Texture2D pauseTexturePath = GD.Load<Texture2D>("res://images/game-map/time/pause.svg");
	private Texture2D speedTexturePath = GD.Load<Texture2D>("res://images/game-map/time/speed.svg");
	private Texture2D speedupTexturePath = GD.Load<Texture2D>("res://images/game-map/time/speedup.svg");
	private Texture2D speedup2TexturePath = GD.Load<Texture2D>("res://images/game-map/time/speedup2.svg");
	
	// Access to autoloaded GDScript singleton
	private Node globals;

	public override void _Ready()
	{
		// Get reference to autoloaded GDScript singleton
		globals = GetNode<Node>("/root/Globals");
		
		// Connect the pressed signal using the generic approach
		// This assumes the script is attached to a button or button-like node
		Connect("pressed", new Callable(this, nameof(OnButtonPressed)));
	}

	private void HandlePlayPause()
	{
		bool gamePaused = globals.Get("gamePaused").AsBool();
		
		if (gamePaused)
		{
			Set("texture_normal", pauseTexturePath);
			globals.Set("gamePaused", false);
			EmitSignal(SignalName.UnpauseGame);
		}
		else
		{
			Set("texture_normal", playTexturePath);
			globals.Set("gamePaused", true);
			EmitSignal(SignalName.PauseGame);
		}
	}

	private void HandleSpeedUp()
	{
		int gameSpeed = globals.Get("gameSpeed").AsInt32();
		
		if (gameSpeed == 1)
		{
			Set("texture_normal", speedupTexturePath);
			globals.Set("gameSpeed", 2);
			GD.Print("Speeding up");
		}
		else if (gameSpeed == 2)
		{
			Set("texture_normal", speedup2TexturePath);
			globals.Set("gameSpeed", 3);
			GD.Print("Speeding up 2");
		}
		else if (gameSpeed == 3)
		{
			globals.Set("gameSpeed", 1);
			Set("texture_normal", speedTexturePath);
			GD.Print("Speeding up 3");
		}
		else
		{
			GD.Print("Speeding up Else");
			globals.Set("gameSpeed", 1);
			Set("texture_normal", speedTexturePath);
		}
		
		EmitSignal(SignalName.SpeedUpGame);
	}

	private void OnButtonPressed()
	{
		string nodeName = Name;
		
		if (nodeName == "PlayPauseBtn")
		{
			HandlePlayPause();
		}
		else if (nodeName == "SpeedUpBtn")
		{
			HandleSpeedUp();
		}
		
		// Uncomment for debugging:
		GD.Print("Current game paused state: " + globals.Get("gamePaused").AsBool());
		GD.Print("Current game speed: " + globals.Get("gameSpeed").AsInt32());
	}
}
