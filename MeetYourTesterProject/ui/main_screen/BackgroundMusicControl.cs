using Godot;

public partial class BackgroundMusicControl : TextureButton
{
    // Access to autoloaded GDScript singleton
    private Node globals;

    public override void _Ready()
    {
        // Get reference to autoloaded GDScript singleton
        globals = GetNode<Node>("/root/Globals");
        
        // Connect the toggled signal
        Toggled += OnToggled;
    }

    private void OnToggled(bool toggledOn)
    {
        if (toggledOn)
        {
            globals.Set("bg_music_volume", 0);
        }
        else
        {
            globals.Set("bg_music_volume", -80);
        }
    }

    public override void _Process(double delta)
    {
    }
}