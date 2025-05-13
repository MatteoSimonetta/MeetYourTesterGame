using Godot;
using System;

public partial class MainScript : Node
{
    [Signal]
    public delegate void GamePauseChangedEventHandler();

    [Signal]
    public delegate void EndGameEventHandler(int type);

    // === Node References ===
    private ExitMenuScene exitMenu;
    private Node mainControl;
    private Node timerControl;
    private Node terminalControl;
    private Node anonymityControlNode;
    private Node progressBarControlNode;
    private GamePauseMenuScene pauseMenu;
    private CloseIngameTutorial tutorialSceneContainer;

    public override void _Ready()
    {
        // OnReady node assignments
        exitMenu = GetNode<ExitMenuScene>("ExitPrompt/exit_menu");
        mainControl = GetNode<Node>("MainControl");
        timerControl = GetNode<Node>("Sprite2D");
        terminalControl = GetNode<Node>("Terminal");
        anonymityControlNode = GetNode<Node>("AnonymityBarControl");
        progressBarControlNode = GetNode<Node>("ProgressBarControl");
        pauseMenu = GetNode<GamePauseMenuScene>("PauseMenu");
        tutorialSceneContainer = GetNode<CloseIngameTutorial>("TutorialSceneContainer");

        // Signal Connections
        exitMenu.Connect("ResumeFromQuitPrompt", new Callable(this, nameof(Resume)));
        progressBarControlNode.Connect("LastDeadlineMissed", new Callable(this, nameof(HandleLastDeadlineMissed)));
        anonymityControlNode.Connect("AnonValueUpdate", new Callable(this, nameof(CheckAnonymityValue)));
        progressBarControlNode.Connect("ProgressBarLimitReached", new Callable(this, nameof(HandleProgressBarLimitReached)));

        pauseMenu.Visible = false;
        pauseMenu.Connect("ResumeGame", new Callable(this, nameof(Resume)));
        tutorialSceneContainer.Connect("ResumeGame", new Callable(this, nameof(Resume)));
        pauseMenu.Connect("OpenTutorial", new Callable(this, nameof(HandleOpenTutorial)));
        pauseMenu.Connect("Quit", new Callable(this, nameof(HandleQuit)));
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("show_prompt"))
        {
            Globals.Instance.GamePaused = true;
            GD.Print("Esc pressed signal emitted");
            EmitSignal(SignalName.GamePauseChanged);
            pauseMenu.Visible = true;

            Utils.Pause(mainControl);
            Utils.Pause(timerControl);
            Utils.Pause(terminalControl);

            ManageHoverNodes();
        }
    }

    private void Resume()
    {
        Globals.Instance.GamePaused = false;

        foreach (Node child in GetChildren())
        {
            if (child is CanvasItem canvasItem)
            {
                canvasItem.Visible = true;
            }
        }

        exitMenu.Visible = false;
        pauseMenu.Visible = false;
        tutorialSceneContainer.Visible = false;

        EmitSignal(SignalName.GamePauseChanged);

        Utils.Unpause(mainControl);
        Utils.Unpause(timerControl);
        Utils.Unpause(terminalControl);

        ManageHoverNodes();
    }

    private void HandleQuit()
    {
        pauseMenu.Visible = false;
        exitMenu.Visible = true;
    }

    private void HandleOpenTutorial()
    {
        pauseMenu.Visible = false;

        foreach (Node child in GetChildren())
        {
            if (child is CanvasItem canvasItem)
            {
                canvasItem.Visible = false;
            }
        }

        tutorialSceneContainer.Visible = true;
    }

    private void HandleLastDeadlineMissed()
    {
        GD.Print("Missed last deadline, you won the game");
        Globals.Instance.EndGameReason = 1;
        GetTree().ChangeSceneToFile("res://ui/menus/end_game_scene.tscn");
    }

    private void CheckAnonymityValue()
    {
        if (Globals.Instance.CurrentAnonymityValue <= 0)
        {
            GD.Print("Current anonymity value reached 0, game lost");
            Globals.Instance.EndGameReason = 2;
            GetTree().ChangeSceneToFile("res://ui/menus/end_game_scene.tscn");
        }
    }

    private void HandleProgressBarLimitReached()
    {
        GD.Print("Progress bar reached its limit, game lost");
        Globals.Instance.EndGameReason = 3;
        GetTree().ChangeSceneToFile("res://ui/menus/end_game_scene.tscn");
    }

    private void ManageHoverNodes()
    {
        GD.Print("HoverNodes");

        string[] subsystems = { "Database", "Delivery", "Business_Logic", "Backend", "UI_UX" };
        foreach (string system in subsystems)
        {
            Node node = mainControl.GetNode<Node>(system);
            node.Call("HandleGameExit", exitMenu.Visible || pauseMenu.Visible);
        }
    }
}
