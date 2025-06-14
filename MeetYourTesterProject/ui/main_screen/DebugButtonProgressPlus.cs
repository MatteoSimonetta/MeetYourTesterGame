using Godot;

public partial class DebugButtonProgressPlus : Button
{
    private readonly Godot.Collections.Dictionary zoneDictPlus = new()
    {
        { "offset", 10 },
        { "speedValue", 1.5 },
        { "length", 2 }
    };
    
    private readonly Godot.Collections.Dictionary zoneDictMinus = new()
    {
        { "offset", 10 },
        { "speedValue", 0.5 },
        { "length", 2 }
    };
    
    private Node gameProgressBar = null;
    private Node anonymityNode = null;

    public override void _Ready()
    {
        gameProgressBar = GetParent().GetNode("GameProgressBar");
        anonymityNode = GetParent().GetParent().GetNode("AnonymityBarControl");
        
        if (gameProgressBar == null)
        {
            GD.PrintErr("game_progress_bar is null");
            GetTree().Quit(1); // Exit with error code
        }
    }

    private void _on_Button_pressed_plus()
    {
        if (gameProgressBar != null)
        {
            double currentValue = gameProgressBar.Get("value").AsDouble();
            gameProgressBar.Set("value", currentValue + 1);
        }
    }

    private void _on_Button_pressed_minus()
    {
        if (gameProgressBar != null)
        {
            double currentValue = gameProgressBar.Get("value").AsDouble();
            gameProgressBar.Set("value", currentValue - 1);
        }
    }

    private void _on_Debug_btn_pressed_plus()
    {
        if (anonymityNode != null)
        {
            anonymityNode.Call("add_anonymity_value", 20);
        }
    }

    private void _on_Debug_btn_pressed_minus()
    {
        if (anonymityNode != null)
        {
            anonymityNode.Call("add_anonymity_value", -20);
        }
    }
}