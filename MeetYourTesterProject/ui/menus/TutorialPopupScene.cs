using Godot;

public partial class TutorialPopupScene : Node2D
{
    private Node popupParent;

    public override void _Ready()
    {
        popupParent = GetNode("../");
    }

    public override void _Process(double delta)
    {
    }

    private void _on_skip_pressed()
    {
        popupParent.Set("visible", false);
        GetTree().ChangeSceneToFile("res://ui/menus/main_menu.tscn");
    }

    private void _on_start_pressed()
    {
        GetTree().ChangeSceneToFile("res://ui/menus/tutorial_scene.tscn");
    }
}