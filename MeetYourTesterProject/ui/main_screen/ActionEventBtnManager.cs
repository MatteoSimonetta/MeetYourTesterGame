using Godot;
using System;
using Godot.Collections;

public partial class ActionEventBtnManager : TextureButton
{
    [ExportCategory("Communication message")]
    [Export]
    public string Message = "Test message";

    private Texture2D backupDisableImage;
    private Texture2D backupHoverImage;
    private bool isActionEventGenerated = false;
    private Timer timerChild;

    [Signal]
    public delegate void HexagonClickedEventHandler(Dictionary paramsDict);

    public override void _Ready()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();

        backupDisableImage = TextureDisabled;
        backupHoverImage = TextureHover;

        // Assuming the Timer node is the first child of the first child
        timerChild = GetChild<Node>(0).GetChild<Timer>(0);
    }

    public override void _Process(double delta)
    {
        // No-op (mirrors GDScript _process pass)
    }

    private void OnTimerTimeout()
    {
        if (!isActionEventGenerated)
        {
            StartSoundSpawnEvent();
            GenerateActionEvent();
        }
        else
        {
            RemoveActionEvent();
        }
    }

    private void _OnPressed()
    {
        TextureDisabled = TexturePressed;
        Disabled = true;

        string nodeName = Name;
        var parameters = new Dictionary
        {
            { "node_name", nodeName }
        };

        EmitSignal(SignalName.HexagonClicked, parameters);
        StartSoundClickedEvent();
    }

    private void GenerateActionEvent()
    {
        Disabled = false;
        isActionEventGenerated = true;

        if (timerChild != null)
        {
            var rng = new RandomNumberGenerator();
            rng.Randomize();
            timerChild.WaitTime = rng.Randi() % Globals.Instance.RandomTimerForActionEventAcceptance;
        }
    }

    private void RemoveActionEvent()
    {
        TextureDisabled = backupDisableImage;
        Disabled = true;
        isActionEventGenerated = false;

        if (timerChild != null)
        {
            timerChild.Stop();

            var rng = new RandomNumberGenerator();
            rng.Randomize();
            timerChild.WaitTime = rng.Randi() % Globals.Instance.RandomTimerForActionEventInactivity;
            timerChild.Start();
        }
    }

    private void StartSoundSpawnEvent()
    {
        var sfx = GetNode<AudioStreamPlayer>("../HexagonActivatedSfx");
        sfx?.Play();
    }

    private void StartSoundClickedEvent()
    {
        var sfx = GetNode<AudioStreamPlayer>("../HexagonClickedSfx");
        sfx?.Play();
    }

    public void HandleGameExit(bool checkGameQuit)
    {
        if (checkGameQuit)
        {
            if (Disabled && isActionEventGenerated)
            {
                TextureHover = TexturePressed;
            }
            else if (Disabled && !isActionEventGenerated)
            {
                TextureHover = TextureDisabled;
            }
            else if (!Disabled)
            {
                TextureHover = TextureNormal;
            }
        }
        else
        {
            TextureHover = backupHoverImage;
        }
    }
}
