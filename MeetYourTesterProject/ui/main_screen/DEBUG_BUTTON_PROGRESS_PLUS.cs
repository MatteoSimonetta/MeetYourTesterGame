using Godot;
using System;
using Godot.Collections;

public partial class DEBUG_BUTTON_PROGRESS_PLUS : Button
{
    private Dictionary zoneDictPlus = new Dictionary
    {
        { "offset", 10 },
        { "speedValue", 1.5f },
        { "length", 2 }
    };

    private Dictionary zoneDictMinus = new Dictionary
    {
        { "offset", 10 },
        { "speedValue", 0.5f },
        { "length", 2 }
    };

    private TextureProgressBar gameProgressBar;
    private Node anonymityNode;

    public override void _Ready()
    {
        gameProgressBar = GetParent().GetNodeOrNull<TextureProgressBar>("GameProgressBar");
        anonymityNode = GetParent().GetParent().GetNodeOrNull<Node>("AnonymityBarControl");

        if (gameProgressBar == null)
        {
            GD.PushError("gameProgressBar is null");
        }
    }

    public void OnButtonPressedPlus()
    {
        if (gameProgressBar != null)
            gameProgressBar.Value += 1;
    }

    public void OnButtonPressedMinus()
    {
        if (gameProgressBar != null)
            gameProgressBar.Value -= 1;
    }

    public void OnDebugBtnPressedPlus()
    {
        anonymityNode?.Call("AddAnonymityValue", 20);
    }

    public void OnDebugBtnPressedMinus()
    {
        anonymityNode?.Call("AddAnonymityValue", -20);
    }
}
