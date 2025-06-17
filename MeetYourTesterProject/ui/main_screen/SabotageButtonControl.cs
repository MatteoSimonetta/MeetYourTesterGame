using Godot;

public partial class SabotageButtonControl : Control
{
    private int chargeCount;
    private Node chargeCountLabel;
    private Node progressBarControlNode;
    private Node anonymityNode;
    private TextureRect chargeOneNode;
    private TextureRect chargeTwoNode;
    private Node anonBar;
    
    // Access to autoloaded GDScript singletons
    private Node globals;
    private Node utils;

    [Signal]
    public delegate void ChargeLimitReachedEventHandler();

    public override void _Ready()
    {
        // Get references to autoloaded GDScript singletons
        globals = GetNode<Node>("/root/Globals");
        utils = GetNode<Node>("/root/Utils");
        
        // Get node references
        chargeCountLabel = FindChild("SabotageChargesLabel");
        progressBarControlNode = GetParent().GetNode("ProgressBarControl");
        anonymityNode = GetParent().GetNode("AnonymityBarControl");
        chargeOneNode = GetNode<TextureRect>("Charge1");
        chargeTwoNode = GetNode<TextureRect>("Charge2");
        anonBar = GetNode("SabotageButton/AnonymityBar");
        
        // Connect signals
        progressBarControlNode.Connect("DeadlineMissed", new Callable(this, nameof(HandleDeadlineMissed)));
        anonymityNode.Connect("AnonValueUpdate", new Callable(this, nameof(HandleAnonValueUpdate)));
        
        chargeCount = 0;
    }

    private void HandleAnonValueUpdate()
    {
        int currentAnonymityValue = globals.Get("current_anonymity_value").AsInt32();
        anonBar.Set("value", currentAnonymityValue);
        
        int sabotageThreshold = globals.Get("ANON_VALUE_SABOTAGE_TRIGGER").AsInt32();
        if (currentAnonymityValue >= sabotageThreshold)
        {
            if (chargeCount > 0)
            {
                GetNode("SabotageButton").Set("disabled", false);
            }
        }
    }

    private void HandleDeadlineMissed()
    {
        // GREAT SUCCESS
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

        chargeCount += 1;
        chargeCountLabel.Call("set_text", chargeCount.ToString());

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
        chargeCountLabel.Call("set_text", newText);
    }

    private void DecreaseChargeCount()
    {
        chargeCount -= 1;
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

    private void _on_sabotage_button_pressed()
    {
        ReduceDeadlineTime();
        DecreaseChargeCount();
        
        int sabotageDecreaseValue = globals.Get("SABOTAGE_ANON_DECREASE_VALUE").AsInt32();
        anonymityNode.Call("AddAnonymityValue", -sabotageDecreaseValue);
        
        if (chargeCount == 0)
        {
            GetNode("SabotageButton").Set("disabled", true);
            return;
        }
    }

    private void ReduceDeadlineTime()
    {
        var deadlinesContainer = progressBarControlNode.Call("find_child", "DeadlinesContainer");
        var deadlinesChildren = ((Node)deadlinesContainer).GetChildren();
        
        foreach (Node deadlineNode in deadlinesChildren)
        {
            var deadlineTexture = (TextureRect)deadlineNode.Call("find_child", "Deadline");
            var deadlineLabel = (Node)deadlineNode.Call("find_child", "DeadlineTimer");
            
            string labelText = deadlineLabel.Get("text").AsString();
            float currentDeadlineTime = utils.Call("time_to_seconds", labelText).AsSingle();
            int decreaseAmount = globals.Get("DECREASE_FINAL_DEADLINE_AMOUNT").AsInt32();
            float newDeadlineTimeAmount = currentDeadlineTime - decreaseAmount;
            
            if (newDeadlineTimeAmount <= 0)
            {
                continue;
            }
            
            string texturePath = deadlineTexture.Texture.ResourcePath;
            if (texturePath.Contains("pending"))
            {
                string newTimeString = utils.Call("float_to_time", newDeadlineTimeAmount).AsString();
                deadlineLabel.Call("set_text", newTimeString);
                break;
            }
        }
    }
}