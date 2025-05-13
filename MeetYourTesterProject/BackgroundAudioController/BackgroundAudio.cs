using Godot;
using System;

public partial class BackgroundAudio : Node
{
    private AudioStreamPlayer music;

    public override void _Ready()
    {
        music = GetNodeOrNull<AudioStreamPlayer>("Music");
        if (music == null)
            GD.PrintErr("Music node not found in BackgroundAudio!");
    }
                                                                                     
    public override void _Process(double delta)
    {
        if (music != null)
            music.VolumeDb = Globals.Instance.BgMusicVolume;
    }
}
