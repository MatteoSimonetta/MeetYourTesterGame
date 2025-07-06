using Godot;

public partial class HandleHover : Node
{
    private TextureButton easyIcon;
    private TextureButton easyLabel;
    private TextureButton mediumIcon;
    private TextureButton mediumLabel;
    private TextureButton hardIcon;
    private TextureButton hardLabel;
    private TextureButton cancelIcon;
    private TextureButton cancelLabel;
    
    // Access to autoloaded GDScript singleton
    private Node globals;

    public override void _Ready()
    {
        // Get reference to autoloaded GDScript singleton
        globals = GetNode<Node>("/root/Globals");
        
        // Get UI elements
        easyIcon = GetNode<TextureButton>("GridContainer2/EasyDiffIcon");
        easyLabel = GetNode<TextureButton>("GridContainer2/CenterContainer/EasyDiffBtn");
        mediumIcon = GetNode<TextureButton>("GridContainer2/MediumDiffIcon");
        mediumLabel = GetNode<TextureButton>("GridContainer2/CenterContainer2/MediumDiffBtn");
        hardIcon = GetNode<TextureButton>("GridContainer2/HardDiffIcon");
        hardLabel = GetNode<TextureButton>("GridContainer2/CenterContainer3/HardDiffBtn");
        cancelIcon = GetNode<TextureButton>("GridContainer2/CancelIcon");
        cancelLabel = GetNode<TextureButton>("GridContainer2/CenterContainer4/CancelBtn");
    }

    public override void _Process(double delta)
    {
        // Empty - keeping for consistency with original
    }

    private void _on_easy_mouse_entered()
    {
        DebugPrint("Easy Game Button (label) on hover entered");
        easyIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-easy-select.svg");
        easyLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-easy-select.svg");
    }

    private void _on_easy_mouse_exited()
    {
        DebugPrint("Easy Game Button (label) on hover exited");
        easyIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-easy.svg");
        easyLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-easy.svg");
    }

    private void _on_medium_mouse_entered()
    {
        DebugPrint("Medium Game Button (label) on hover entered");
        mediumIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-medium-select.svg");
        mediumLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-medium-select.svg");
    }

    private void _on_medium_mouse_exited()
    {
        DebugPrint("Medium Game Button (label) on hover exited");
        mediumIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-medium.svg");
        mediumLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-medium.svg");
    }

    private void _on_hard_mouse_entered()
    {
        DebugPrint("Hard Game Button (label) on hover entered");
        hardIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-hard-select.svg");
        hardLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-hard-select.svg");
    }

    private void _on_hard_mouse_exited()
    {
        DebugPrint("Hard Game Button (label) on hover exited");
        hardIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-hard.svg");
        hardLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-hard.svg");
    }

    private void _on_cancel_mouse_entered()
    {
        DebugPrint("Cancel Game Button (label) on hover entered");
        cancelIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-cancel-select.svg");
        cancelLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-cancel-select.svg");
    }

    private void _on_cancel_mouse_exited()
    {
        DebugPrint("Cancel Game Button (label) on hover exited");
        cancelIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-cancel.svg");
        cancelLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-cancel.svg");
    }

    private void DebugPrint(string msg)
    {
        // Access DEBUG_MODE from the Globals GDScript autoload
        bool debugMode = globals.Get("DEBUG_MODE").AsBool();
        if (debugMode)
        {
            GD.Print(msg);
        }
    }
}