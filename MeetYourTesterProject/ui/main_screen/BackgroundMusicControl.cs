using Godot;
using System;

public partial class BackgroundMusicControl : TextureButton
{
    public override void _Ready()
    {
        this.Toggled += OnToggled;
    }

    private void OnToggled(bool toggledOn)
    {
        Globals.Instance.BgMusicVolume = toggledOn ? 0f : -80f;
    }
}
