using Godot;
using System;

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

    public override void _Ready()
    {
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
    }

    private void OnEasyMouseEntered()
    {
        DebugPrint("Easy Game Button (label) on hover entered");
        easyIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-icon-easy-select.svg");
        easyLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-label-easy-select.svg");
    }

    private void OnEasyMouseExited()
    {
        DebugPrint("Easy Game Button (label) on hover exited");
        easyIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-icon-easy.svg");
        easyLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-label-easy.svg");
    }

    private void OnMediumMouseEntered()
    {
        DebugPrint("Medium Game Button (label) on hover entered");
        mediumIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-icon-medium-select.svg");
        mediumLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-label-medium-select.svg");
    }

    private void OnMediumMouseExited()
    {
        DebugPrint("Medium Game Button (label) on hover exited");
        mediumIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-icon-medium.svg");
        mediumLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-label-medium.svg");
    }

    private void OnHardMouseEntered()
    {
        DebugPrint("Hard Game Button (label) on hover entered");
        hardIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-icon-hard-select.svg");
        hardLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-label-hard-select.svg");
    }

    private void OnHardMouseExited()
    {
        DebugPrint("Hard Game Button (label) on hover exited");
        hardIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-icon-hard.svg");
        hardLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-label-hard.svg");
    }

    private void OnCancelMouseEntered()
    {
        DebugPrint("Cancel Game Button (label) on hover entered");
        cancelIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-icon-cancel-select.svg");
        cancelLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-label-cancel-select.svg");
    }

    private void OnCancelMouseExited()
    {
        DebugPrint("Cancel Game Button (label) on hover exited");
        cancelIcon.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-icon-cancel.svg");
        cancelLabel.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/start-scene/btn-label-cancel.svg");
    }

    private void DebugPrint(string msg)
    {
        if (Globals.DEBUG_MODE)
            GD.Print(msg);
    }
}
