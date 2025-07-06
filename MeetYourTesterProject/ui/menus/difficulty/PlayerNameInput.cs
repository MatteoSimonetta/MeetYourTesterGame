using Godot;

public partial class PlayerNameInput : LineEdit
{
    [Signal]
    public delegate void EmptyNameEventHandler(bool isEmpty);

    public override void _Ready()
    {
        // Connect the text_changed signal
        TextChanged += OnLineEditTextEntered;
    }

    public override void _Process(double delta)
    {
        // Empty - keeping for consistency with original
    }

    private void OnLineEditTextEntered(string newText)
    {
        // Sanitize input
        string escapedText = newText.CEscape();
        
        GD.Print("Text entered: ", escapedText);
        
        if (string.IsNullOrEmpty(escapedText))
        {
            EmitSignal(SignalName.EmptyName, true);
            escapedText = "Player";
        }
        else
        {
            EmitSignal(SignalName.EmptyName, false);
        }
        
        // DiffStore.player_name = escapedText;
    }
}