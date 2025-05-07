using Godot;
using System;

public partial class TimerController : Node
{
    private Node progressBar;
    private Node mainGameScene;
    private Timer timerNode;

    public override void _Ready()
    {
        progressBar = GetNode("../../../ProgressBarControl");
        mainGameScene = GetNode("../../..");
        timerNode = GetNode<Timer>("TimerNode");

        mainGameScene.Connect("GamePauseChanged", new Callable(this, nameof(CatchPauseChanged)));
    }

    // This function is assumed to be connected in the editor to the timeout() signal of TimerNode
    private void OnTimerNodeTimeout()
    {
        Globals.Instance.GameTime += 1;
        progressBar.Call("auto_increment");
        this.Set("text", Utils.FloatToTime(Globals.Instance.GameTime));
    }

    private void CatchPauseChanged()
    {
        if (Globals.Instance.GamePaused)
            CatchPause();
        else
            CatchUnpause();
    }

    private void CatchPause()
    {
        timerNode.Paused = true;
    }

    private void CatchUnpause()
    {
        timerNode.Paused = false;
    }

    public void CatchSpeedChange()
    {
        Engine.TimeScale = Globals.Instance.GameSpeed;
    }
}
