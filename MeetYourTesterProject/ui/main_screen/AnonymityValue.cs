using Godot;
using System;
using Godot.Collections;

public partial class AnonymityValue : Control
{
    [Signal]
    public delegate void AnonValueUpdateEventHandler();

    private Node terminal;

    public override void _Ready()
    {
        terminal = GetNode("../Terminal/_terminal_mock/terminal_content");
        terminal.Connect("answer_signal", new Callable(this, nameof(HandleAnswerSignal)));
    }

    private void HandleAnswerSignal(Dictionary selectedAnswer)
    {
        if (selectedAnswer.ContainsKey(Globals.ANONYMITY_BAR_DICTIONARY_KEY))
        {
            var valueContainer = (Dictionary)selectedAnswer[Globals.ANONYMITY_BAR_DICTIONARY_KEY];
            int value = (int)valueContainer["value"];
            AddAnonymityValue(value);

            if (Globals.Instance.CurrentAnonymityValue > Globals.Instance.AnonymityValueAlertThreshold)
            {
                GD.Print("Anonymity bar threshold passed");
            }
        }
    }

    public void AddAnonymityValue(int value)
    {
        Globals.Instance.CurrentAnonymityValue += value;

        if (Globals.Instance.CurrentAnonymityValue > Globals.Instance.MaxAnonimityValue)
        {
            Globals.Instance.CurrentAnonymityValue = Globals.Instance.MaxAnonimityValue;
        }

        GetChild<Label>(0).Text = $"Anonimity: {Globals.Instance.CurrentAnonymityValue}";
        EmitSignal(SignalName.AnonValueUpdate);
    }
}
