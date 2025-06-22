using Godot;

public partial class MainScript : Node
{
    [Signal]
    public delegate void GamePauseChangedEventHandler();
    
    [Signal]
    public delegate void EndGameEventHandler(int type);

    private Node exitMenu;
    private Node mainControl;
    private Node timerControl;
    private Node terminalControl;
    private Node anonymityControlNode;
    private Node progressBarControlNode;
    private Node pauseMenu;
    private Node tutorialSceneContainer;
    
    // Access to autoloaded GDScript singletons
    private Node globals;
    private Node utils;

    public override void _Ready()
    {
        // Get references to autoloaded GDScript singletons
        globals = GetNode<Node>("/root/Globals");
        utils = GetNode<Node>("/root/Utils");
        
        // Get node references
        exitMenu = GetNode("ExitPrompt/exit_menu");
        mainControl = GetNode("MainControl");
        timerControl = GetNode("Sprite2D");
        terminalControl = GetNode("Terminal");
        anonymityControlNode = GetNode("AnonymityBarControl");
        progressBarControlNode = GetNode("ProgressBarControl");
        pauseMenu = GetNode("PauseMenu");
        tutorialSceneContainer = GetNode("TutorialSceneContainer");
        
        // Connect signals
        exitMenu.Connect("ResumeFromQuitPrompt", new Callable(this, nameof(Resume)));
        progressBarControlNode.Connect("LastDeadlineMissed", new Callable(this, nameof(HandleLastDeadlineMissed)));
        anonymityControlNode.Connect("AnonValueUpdate", new Callable(this, nameof(CheckAnonymityValue)));
        progressBarControlNode.Connect("ProgressBarLimitReached", new Callable(this, nameof(HandleProgressBarLimitReached)));
        
        pauseMenu.Set("visible", false);
        pauseMenu.Connect("ResumeGame", new Callable(this, nameof(Resume)));
        tutorialSceneContainer.Connect("ResumeGame", new Callable(this, nameof(Resume)));
        pauseMenu.Connect("OpenTutorial", new Callable(this, nameof(HandleOpenTutorial)));
        pauseMenu.Connect("Quit", new Callable(this, nameof(HandleQuit)));

        // To activate music every time the game start
        globals.Set("bg_music_volume", 0);
        globals.Set("sound_fx_volume", -10);
    }

    private void HandleQuit()
    {
        pauseMenu.Set("visible", false);
        exitMenu.Set("visible", true);
    }

    private void HandleOpenTutorial()
    {
        pauseMenu.Set("visible", false);
        
        var children = GetChildren();
        
        for (int i = 0; i < children.Count; i++)
        {
            children[i].Set("visible", false);
        }
        
        tutorialSceneContainer.Set("visible", true);
    }

    private void HandleLastDeadlineMissed()
    {
        GD.Print("Missed last deadline, you won the game");
        globals.Set("end_game_reason", 1);
        GetTree().ChangeSceneToFile("res://ui/menus/end_game_scene.tscn");
    }

    private void CheckAnonymityValue()
    {
        int currentAnonymityValue = globals.Get("current_anonymity_value").AsInt32();
        if (currentAnonymityValue <= 0)
        {
            GD.Print("Current anonymity value reached 0, game lost");
            globals.Set("end_game_reason", 2);
            GetTree().ChangeSceneToFile("res://ui/menus/end_game_scene.tscn");
        }
    }

    private void HandleProgressBarLimitReached()
    {
        GD.Print("Progress bar reached its limit, game lost");
        globals.Set("end_game_reason", 3);
        GetTree().ChangeSceneToFile("res://ui/menus/end_game_scene.tscn");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("show_prompt"))
        {
            globals.Set("gamePaused", true);
            GD.Print("Esc pressed signal emitted");
            EmitSignal(SignalName.GamePauseChanged);
            pauseMenu.Set("visible", true);
            utils.Call("pause", mainControl);
            utils.Call("pause", timerControl);
            utils.Call("pause", terminalControl);
            ManageHoverNodes();
        }
    }

    private void Resume()
    {
        globals.Set("gamePaused", false);
        
        var children = GetChildren();
        
        for (int i = 0; i < children.Count; i++)
        {
            children[i].Set("visible", true);
        }
        
        exitMenu.Set("visible", false);
        pauseMenu.Set("visible", false);
        tutorialSceneContainer.Set("visible", false);
        EmitSignal(SignalName.GamePauseChanged);
        utils.Call("unpause", mainControl);
        utils.Call("unpause", timerControl);
        utils.Call("unpause", terminalControl);
        ManageHoverNodes();
    }

    private void ManageHoverNodes()
    {
        GD.Print("HoverNodes");
        bool menuVisible = exitMenu.Get("visible").AsBool() || pauseMenu.Get("visible").AsBool();
        
        GetNode("MainControl/Database").Call("HandleGameExit", menuVisible);
        GetNode("MainControl/Delivery").Call("HandleGameExit", menuVisible);
        GetNode("MainControl/Business_Logic").Call("HandleGameExit", menuVisible);
        GetNode("MainControl/Backend").Call("HandleGameExit", menuVisible);
        GetNode("MainControl/UI_UX").Call("HandleGameExit", menuVisible);
    }
}