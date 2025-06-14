using Godot;

public partial class SoundEffectControl : TextureButton
{
    private AudioStreamPlayer soundClicked;
    private AudioStreamPlayer soundActivated;
    private AudioStreamPlayer soundSelected;
    private Node globals;

    public override void _Ready()
    {
        // Get reference to autoloaded GDScript singleton
        globals = GetNode<Node>("/root/Globals");
        
        // Get audio stream players
        soundClicked = GetNode<AudioStreamPlayer>("../../MainControl/HexagonClickedSfx");
        soundActivated = GetNode<AudioStreamPlayer>("../../MainControl/HexagonActivatedSfx");
        soundSelected = GetNode<AudioStreamPlayer>("../../Terminal/_terminal_mock/TerminalSelectedAnswerSfx");
        
        // Connect the toggled signal
        Toggled += OnToggled;
    }

    private void OnToggled(bool toggledOn)
    {
        if (toggledOn)
        {
            soundClicked.VolumeDb = -10;
            soundActivated.VolumeDb = -10;
            soundSelected.VolumeDb = -10;
            globals.Set("sound_fx_volume", -10);
        }
        else
        {
            soundClicked.VolumeDb = -80;
            soundActivated.VolumeDb = -80;
            soundSelected.VolumeDb = -80;
            globals.Set("sound_fx_volume", -80);
        }
    }

    public override void _Process(double delta)
    {
    }
}