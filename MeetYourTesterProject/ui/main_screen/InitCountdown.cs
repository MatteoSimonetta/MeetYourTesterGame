using Godot;
using System;

public partial class InitCountdown : Timer
{
	private TextureButton playPauseBtn;
	private TextureButton speedUpBtn;
	private Node terminal;
	private Node mainGameScene;
	private Node hexParent;

	private bool actionEventFlagPause = false;

	public override void _Ready()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Randomize();
		WaitTime = rng.Randi() % Globals.Instance.RandomTimerForActionEventInactivity;
		Start();

		playPauseBtn = GetNode<TextureButton>("../../../../Sprite2D/TimerContainer/PlayPauseBtn");
		speedUpBtn = GetNode<TextureButton>("../../../../Sprite2D/TimerContainer/SpeedUpBtn");
		terminal = GetNode("../../../../Terminal/_terminal_mock/terminal_content");
		mainGameScene = GetNode("../../../..");
		hexParent = GetParent().GetParent();

		playPauseBtn.Connect("PauseGame", new Callable(this, nameof(StopResumeTimer)));
		playPauseBtn.Connect("UnpauseGame", new Callable(this, nameof(StopResumeTimer)));
		mainGameScene.Connect("GamePauseChanged", new Callable(this, nameof(StopResumeTimer)));
		terminal.Connect("AnswerSignal", new Callable(this, nameof(HandleAnswerStopResume)));
		hexParent.Connect("HexagonClicked", new Callable(this, nameof(HandleHexagonClick)));
	}

	public override void _Process(double delta)
	{
		if (Globals.DEBUG_MODE)
		{
			SendTimeToLabel();
		}
	}

	private void HandleAnswerStopResume(Godot.Collections.Dictionary answerImpact)
	{
		if (answerImpact["node_name"].ToString() == hexParent.Name)
		{
			actionEventFlagPause = false;
			StopResumeTimer();
			GD.Print(hexParent.Name);
		}
	}

	private void HandleHexagonClick(Variant _params)
	{
		actionEventFlagPause = true;
		StopResumeTimer();
	}

	private void StopResumeTimer()
	{
		SetPaused(Globals.Instance.GamePaused || actionEventFlagPause);
	}

	private void SendTimeToLabel()
	{
		string subStr;
		if (TimeLeft > 9)
		{
			subStr = TimeLeft.ToString("0.00").Substring(0, 2);
		}
		else
		{
			subStr = TimeLeft.ToString("0.00").Substring(0, 3);
		}

		if (GetParent() is Label label)
		{
			label.Text = subStr;
		}
	}
}
