using Godot;
using System;

public partial class BackgroundAudio : Node
{
    public override void _Process(double delta)
    {
        var music = GetNode<AudioStreamPlayer>("Music");
        music.VolumeDb = Globals.Instance.BgMusicVolume;
    }
}
