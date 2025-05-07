using Godot;
using System;

public partial class TimerBtns : TextureButton
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

    public override void _Ready()
    {
        Pressed += OnButtonPressed;
    }

    private void HandlePlayPause()
    {
        if (Globals.Instance.GamePaused)
        {
            TextureNormal = pauseTexturePath;
            Globals.Instance.GamePaused = false;
            EmitSignal(SignalName.UnpauseGame);
        }
        else
        {
            TextureNormal = playTexturePath;
            Globals.Instance.GamePaused = true;
            EmitSignal(SignalName.PauseGame);
        }
    }

    private void HandleSpeedUp()
    {
        if (Globals.Instance.GameSpeed == 1)
        {
            TextureNormal = speedupTexturePath;
            Globals.Instance.GameSpeed = 2;
            GD.Print("Speeding up");
        }
        else if (Globals.Instance.GameSpeed == 2)
        {
            TextureNormal = speedup2TexturePath;
            Globals.Instance.GameSpeed = 3;
            GD.Print("Speeding up 2");
        }
        else if (Globals.Instance.GameSpeed == 3)
        {
            Globals.Instance.GameSpeed = 1;
            TextureNormal = speedTexturePath;
            GD.Print("Speeding up 3");
        }
        else
        {
            GD.Print("Speeding up Else");
            Globals.Instance.GameSpeed = 1;
            TextureNormal = speedTexturePath;
        }

        EmitSignal(SignalName.SpeedUpGame);
    }

    private void OnButtonPressed()
    {
        if (Name == "PlayPauseBtn")
        {
            HandlePlayPause();
        }
        else if (Name == "SpeedUpBtn")
        {
            HandleSpeedUp();
        }

        // Debug info (optional):
        // GD.Print($"Paused: {Globals.Instance.GamePaused}, Speed: {Globals.Instance.GameSpeed}");
    }
}
