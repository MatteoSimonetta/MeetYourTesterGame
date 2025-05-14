using Godot;
using System;

public partial class TutorialPopupScene : Node2D
{
    private CanvasItem popupParent;

    public override void _Ready()
    {
        popupParent = GetNode<CanvasItem>("../");
    }

    public override void _Process(double delta)
    {
    }

    private void OnSkipPressed()
    {
        popupParent.Visible = false;
        GetTree().ChangeSceneToFile("res://ui/menus/main_menu.tscn");
    }

    private void OnStartPressed()
    {
        GetTree().ChangeSceneToFile("res://ui/menus/tutorial_scene.tscn");
    }
}
