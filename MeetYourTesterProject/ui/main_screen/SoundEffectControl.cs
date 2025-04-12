using Godot;
using System;

public partial class SoundEffectControl : TextureButton
{
    private AudioStreamPlayer soundClicked;
    private AudioStreamPlayer soundSelected;
    private AudioStreamPlayer soundActivated;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        soundClicked = GetNode<AudioStreamPlayer>("../../MainControl/HexagonClickedSfx");
	    soundActivated = GetNode<AudioStreamPlayer>("../../MainControl/HexagonActivatedSfx");
	    soundSelected = GetNode<AudioStreamPlayer>("../../Terminal/_terminal_mock/TerminalSelectedAnswerSfx");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void _toggled(Boolean toggled_on) // da evitare di ripetere piu` volte tutti i valori
    {
        if (toggled_on)
        {
            soundClicked.VolumeDb = -10f;
            soundActivated.VolumeDb = -10f;
            soundSelected.VolumeDb = -10f;
            Globals.Instance.SoundFxVolume = -10f;
        }
        else
        {
            soundClicked.VolumeDb = -80f;
            soundActivated.VolumeDb = -80f;
            soundSelected.VolumeDb = -80f;
            Globals.Instance.SoundFxVolume = -80f;
        }
    }
}
