using Godot;

public partial class InitCountdown : Timer
{
	private Node playPauseBtn;
	private Node speedUpBtn;
	private Node terminal;
	private Node mainGameScene;
	private Node hexParent;
	
	private bool actionEventFlagPause = false;
	
	// Access to autoloaded GDScript singleton
	private Node globals;

	public override void _Ready()
	{
		// Get reference to autoloaded GDScript singleton
		globals = GetNode<Node>("/root/Globals");
		
		// Get node references
		playPauseBtn = GetNode("../../../../Sprite2D/TimerContainer/PlayPauseBtn");
		speedUpBtn = GetNode("../../../../Sprite2D/TimerContainer/SpeedUpBtn");
		terminal = GetNode("../../../../Terminal/_terminal_mock/terminal_content");
		mainGameScene = GetNode("../../../..");
		hexParent = GetParent().GetParent();
		
		// Initialize random timer
		var random = new RandomNumberGenerator();
		random.Randomize();
		int randomTimerValue = globals.Get("randomTimerForActionEventInactivity").AsInt32();
		Set("wait_time", random.RandiRange(0, randomTimerValue));
		Call("start");
		
		// Connect signals
		playPauseBtn.Connect("PauseGame", new Callable(this, nameof(StopResumeTimer)));
		mainGameScene.Connect("game_pause_changed", new Callable(this, nameof(StopResumeTimer)));
		playPauseBtn.Connect("UnpauseGame", new Callable(this, nameof(StopResumeTimer)));
		terminal.Connect("AnswerSignal", new Callable(this, nameof(HandleAnswerStopResume)));
		hexParent.Connect("HexagonClicked", new Callable(this, nameof(HandleHexagonClick)));
	}

	public override void _Process(double delta)
	{
		bool debugMode = globals.Get("DEBUG_MODE").AsBool();
		if (debugMode)
		{
			SendTimeToLabel();
		}
	}

	private void HandleAnswerStopResume(Godot.Collections.Dictionary answerImpact)
	{
		string hexParentName = hexParent.Get("name").AsString();
		string answerNodeName = answerImpact["node_name"].AsString();
		
		if (answerNodeName == hexParentName)
		{
			actionEventFlagPause = false;
			StopResumeTimer();
			GD.Print(hexParentName);
		}
	}

	private void HandleHexagonClick(Variant parameters)
	{
		actionEventFlagPause = true;
		StopResumeTimer();
	}

	private void StopResumeTimer()
	{
		bool gamePaused = globals.Get("gamePaused").AsBool();
		Call("set_paused", gamePaused || actionEventFlagPause);
	}

	private void SendTimeToLabel()
	{
		string subStr = "";
		float timeLeft = Get("time_left").AsSingle();
		
		if (timeLeft > 9)
		{
			subStr = timeLeft.ToString().Substring(0, 2);
		}
		else
		{
			string timeStr = timeLeft.ToString();
			int length = Mathf.Min(3, timeStr.Length);
			subStr = timeStr.Substring(0, length);
		}
		
		GetParent().Set("text", subStr);
	}
}
