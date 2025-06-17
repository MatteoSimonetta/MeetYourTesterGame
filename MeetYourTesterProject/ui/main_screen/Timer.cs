using Godot;

public partial class Timer : Node
{
    private Node progressBar;
    private Node mainGameScene;
    
    // Access to autoloaded GDScript singletons
    private Node globals;
    private Node utils;

    public override void _Ready()
    {
        // Get references to autoloaded GDScript singletons
        globals = GetNode<Node>("/root/Globals");
        utils = GetNode<Node>("/root/Utils");
        
        // Get node references
        progressBar = GetNode("../../../ProgressBarControl");
        mainGameScene = GetNode("../../..");
        
        // Connect to the game pause changed signal
        mainGameScene.Connect("GamePauseChanged", new Callable(this, nameof(CatchPauseChanged)));
    }

    // Connected via Editor
    private void _on_timer_node_timeout()
    {
        // Increment game time
        int gameTime = globals.Get("gameTime").AsInt32();
        globals.Set("gameTime", gameTime + 1);
        
        // Increment progress bar value
        progressBar.Call("AutoIncrement");
        
        // Update timer display text using Utils
        float currentGameTime = globals.Get("gameTime").AsSingle();
        string timeString = utils.Call("float_to_time", currentGameTime).AsString();
        Set("text", timeString);
    }

    private void CatchPauseChanged()
    {
        bool gamePaused = globals.Get("gamePaused").AsBool();
        
        if (gamePaused)
        {
            CatchPause();
        }
        else
        {
            CatchUnpause();
        }
    }

    private void CatchPause()
    {
        var timerNode = GetNode("TimerNode");
        timerNode.Set("paused", true);
    }

    private void CatchUnpause()
    {
        var timerNode = GetNode("TimerNode");
        timerNode.Set("paused", false);
    }

    private void CatchSpeedChange()
    {
        int gameSpeed = globals.Get("gameSpeed").AsInt32();
        Engine.TimeScale = gameSpeed;
    }
}