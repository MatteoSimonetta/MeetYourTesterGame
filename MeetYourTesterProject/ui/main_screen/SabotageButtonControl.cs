using Godot;
using System;
using Godot.Collections;

public partial class SabotageButtonControl : Control
{
    [Signal]
    public delegate void ChargeLimitReachedEventHandler();

    private int chargeCount;

    private Label chargeCountLabel;
    private Node progressBarControlNode;
    private Node anonimityNode;
    private TextureRect chargeOneNode;
    private TextureRect chargeTwoNode;
    private TextureProgressBar anonBar;

    public override void _Ready()
    {
        chargeCountLabel = GetNode<Label>("SabotageChargesLabel");
        progressBarControlNode = GetParent().GetNode("ProgressBarControl");
        anonimityNode = GetParent().GetNode("AnonymityBarControl");
        chargeOneNode = GetNode<TextureRect>("Charge1");
        chargeTwoNode = GetNode<TextureRect>("Charge2");
        anonBar = GetNode<TextureProgressBar>("SabotageButton/AnonymityBar");

        progressBarControlNode.Connect("DeadlineMissed", new Callable(this, nameof(HandleDeadlineMissed)));
        anonimityNode.Connect("AnonValueUpdate", new Callable(this, nameof(HandleAnonValueUpdate)));

        chargeCount = 0;
    }

    private void HandleAnonValueUpdate()
    {
        anonBar.Value = Globals.Instance.CurrentAnonymityValue;

        if (Globals.Instance.CurrentAnonymityValue >= Globals.ANON_VALUE_SABOTAGE_TRIGGER)
        {
            if (chargeCount > 0)
            {
                GetNode<Button>("SabotageButton").Disabled = false;
            }
        }
    }

    private void HandleDeadlineMissed()
    {
        GD.Print("Got a charge");
        IncreaseChargeCount();
        ChangeLabelText(chargeCount.ToString());
        HandleAnonValueUpdate();
    }

    private void IncreaseChargeCount()
    {
        if (chargeCount == 2)
        {
            GD.Print("You have enough charges");
            EmitSignal(SignalName.ChargeLimitReached);
            return;
        }

        chargeCount++;
        chargeCountLabel.Text = chargeCount.ToString();

        if (chargeCount == 1)
        {
            chargeOneNode.Texture = GD.Load<Texture2D>("res://images/sabotage-buttons/charge-filled.svg");
        }
        else
        {
            chargeTwoNode.Texture = GD.Load<Texture2D>("res://images/sabotage-buttons/charge-filled.svg");
        }
    }

    private void ChangeLabelText(string newText)
    {
        chargeCountLabel.Text = newText;
    }

    private void DecreaseChargeCount()
    {
        chargeCount--;
        ChangeLabelText(chargeCount.ToString());

        if (chargeCount == 0)
        {
            chargeOneNode.Texture = GD.Load<Texture2D>("res://images/sabotage-buttons/charge-empty.svg");
        }
        else
        {
            chargeTwoNode.Texture = GD.Load<Texture2D>("res://images/sabotage-buttons/charge-empty.svg");
        }
    }

    private void OnSabotageButtonPressed()
    {
        ReduceDeadlineTime();
        DecreaseChargeCount();
        anonimityNode.Call("add_anonymity_value", -Globals.SABOTAGE_ANON_DECREASE_VALUE);

        if (chargeCount == 0)
        {
            GetNode<Button>("SabotageButton").Disabled = true;
        }
    }

    private void ReduceDeadlineTime()
    {
        var deadlinesContainer = progressBarControlNode.FindChild("DeadlinesContainer") as Node;

        foreach (var child in deadlinesContainer.GetChildren())
        {
            var deadlineNode = child as Node;
            var deadlineTexture = deadlineNode.FindChild("Deadline") as TextureRect;
            var deadlineLabel = deadlineNode.FindChild("DeadlineTimer") as Label;

            float newDeadlineTime = Utils.TimeToSeconds(deadlineLabel.Text) - Globals.Instance.DecreaseFinalDeadlineAmount;

            if (newDeadlineTime <= 0)
            {
                continue;
            }

            if (deadlineTexture.Texture.ResourcePath.Contains("pending"))
            {
                deadlineLabel.Text = Utils.FloatToTime(newDeadlineTime);
                break;
            }
        }
    }
}
