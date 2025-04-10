using Godot;
using System;

public partial class GamePauseMenuScene : Node2D
{
    [Signal]
    public delegate void ResumeGameEventHandler();
    [Signal]
    public delegate void OpenTutorialEventHandler();
    [Signal]
    public delegate void QuitEventHandler();

    private TextureButton startIcon;
    private TextureButton startLabel;
    private TextureButton quitIcon;
    private TextureButton quitLabel;
    private TextureButton tutorialIcon;
    private TextureButton tutorialLabel;


    public override void _Ready()
    {
        startIcon = GetNode<TextureButton>("GridContainer/StartIcon");
        startLabel = GetNode<TextureButton>("GridContainer/CenterStartLabel/StartLabel");

        quitIcon = GetNode<TextureButton>("GridContainer/QuitIcon");
        quitLabel = GetNode<TextureButton>("GridContainer/CenterQuitLabel/QuitLabel");

        tutorialIcon = GetNode<TextureButton>("GridContainer/TutorialIcon");
        tutorialLabel = GetNode<TextureButton>("GridContainer/CenterTutorialLabel/TutorialLabel");

        // This is how you can connect signals without using GUI
        // Resume button
        startIcon.Pressed += OnResumeGamePressed;
        startLabel.Pressed += OnResumeGamePressed;

        startIcon.MouseEntered += OnStartIconMouseEntered;
        startIcon.MouseExited += OnStartIconMouseExited;
        startLabel.MouseEntered += OnStartLabelMouseEntered;
        startLabel.MouseExited += OnStartLabelMouseExited;

        // Quit button
        quitIcon.Pressed += OnQuitPressed;
        quitLabel.Pressed += OnQuitPressed;

        quitIcon.MouseEntered += OnQuitIconMouseEntered;
        quitIcon.MouseExited += OnQuitIconMouseExited;
        quitLabel.MouseEntered += OnQuitLabelMouseEntered;
        quitLabel.MouseExited += OnQuitLabelMouseExited;

        // Tutorial button
        tutorialIcon.Pressed += OnStartTutorialPressed;
        tutorialLabel.Pressed += OnStartTutorialPressed;

        tutorialIcon.MouseEntered += OnTutorialIconMouseEntered;
        tutorialIcon.MouseExited += OnTutorialIconMouseExited;
        tutorialLabel.MouseEntered += OnTutorialLabelMouseEntered;
        tutorialLabel.MouseExited += OnTutorialLabelMouseExited;
    }

    public override void _Process(double delta)
    {
    }

    private void OnResumeGamePressed()
    {
        DebugPrint("Resume game");
        EmitSignal(SignalName.ResumeGame);
    }

    private void OnStartTutorialPressed()
    {
        DebugPrint("Start tutorial");
        EmitSignal(SignalName.OpenTutorial);
    }

    private void OnQuitPressed()
    {
        DebugPrint("Quit");
        EmitSignal(SignalName.Quit);
    }

    private void OnStartLabelMouseEntered()
    {
        DebugPrint("Start Game Button (label) on hover entered");
        startIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-icon-return-select.svg");
    }

    private void OnStartLabelMouseExited()
    {
        DebugPrint("Start Game Button (label) on hover exited");
        startIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-icon-return.svg");
    }

    private void OnStartIconMouseEntered()
    {
        DebugPrint("Start Game Button (icon) on hover entered");
        startLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-label-return-select.svg");
    }

    private void OnStartIconMouseExited()
    {
        DebugPrint("Start Game Button (icon) on hover exited");
        startLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-label-return.svg");
    }

    private void OnQuitIconMouseEntered()
    {
        DebugPrint("Quit Game Button (icon) on hover entered");
        quitLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-label-quit-select.svg");
    }

    private void OnQuitIconMouseExited()
    {
        DebugPrint("Quit Game Button (icon) on hover exited");
        quitLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-label-quit.svg");
    }

    private void OnQuitLabelMouseEntered()
    {
        DebugPrint("Quit Game Button (label) on hover entered");
        quitIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-icon-quit-select.svg");
    }

    private void OnQuitLabelMouseExited()
    {
        DebugPrint("Quit Game Button (label) on hover exited");
        quitIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-icon-quit.svg");
    }

    private void OnTutorialIconMouseEntered()
    {
        DebugPrint("Tutorial Game Button (icon) on hover entered");
        tutorialLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-label-tutorial-select.svg");
    }

    private void OnTutorialIconMouseExited()
    {
        DebugPrint("Tutorial Game Button (icon) on hover exited");
        tutorialLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-label-tutorial.svg");
    }

    private void OnTutorialLabelMouseEntered()
    {
        DebugPrint("Tutorial Game Button (label) on hover entered");
        tutorialIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-icon-tutorial-select.svg");
    }

    private void OnTutorialLabelMouseExited()
    {
        DebugPrint("Tutorial Game Button (label) on hover exited");
        tutorialIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/pause-menu/btn-icon-tutorial.svg");
    }

    private void DebugPrint(string msg)
    {
        if ((bool)ProjectSettings.GetSetting("debug/mode"))
        {
            GD.Print(msg);
        }
    }
}
