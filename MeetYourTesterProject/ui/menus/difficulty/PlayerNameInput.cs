using Godot;
using System;

public partial class PlayerNameInput : LineEdit
{
    [Signal]
    public delegate void EmptyNameEventHandler(bool isEmpty);

    public override void _Ready()
    {
        TextChanged += OnLineEditTextEntered;
    }

    private void OnLineEditTextEntered(string newText)
    {
        var escapedText = StringExtensions.CEscape(newText);

        GD.Print("Text entered: ", escapedText);

        if (escapedText == "")
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

    public override void _Process(double delta)
    {
    }
}
