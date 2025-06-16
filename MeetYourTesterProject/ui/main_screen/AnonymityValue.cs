using Godot;

public partial class AnonymityValue : Control
{
    private Node terminal;
    
    // Access to autoloaded GDScript singleton
    private Node globals;

    [Signal]
    public delegate void AnonValueUpdateEventHandler();

    public override void _Ready()
    {
        // Get reference to autoloaded GDScript singleton
        globals = GetNode<Node>("/root/Globals");
        
        // Get terminal reference
        terminal = GetNode("../Terminal/_terminal_mock/terminal_content");
        
        // Connect to the answer signal
        terminal.Connect("AnswerSignal", new Callable(this, nameof(HandleAnswerSignal)));
    }

    private void HandleAnswerSignal(Godot.Collections.Dictionary selectedAnswer)
    {
        // Check if the signal is for anonymity bar
        string anonymityBarKey = globals.Get("ANONYMITY_BAR_DICTIONARY_KEY").AsString();
        
        if (selectedAnswer.ContainsKey(anonymityBarKey))
        {
            // Expose value to parse it globally
            var anonymityData = selectedAnswer[anonymityBarKey].AsGodotDictionary();
            int value = anonymityData["value"].AsInt32();
            AddAnonymityValue(value);
            
            int currentAnonymityValue = globals.Get("current_anonymity_value").AsInt32();
            int alertThreshold = globals.Get("anonymity_value_alert_threshold").AsInt32();
            
            if (currentAnonymityValue > alertThreshold)
            {
                GD.Print("Anonymity bar threshold passed");
            }
        }
    }

    public void AddAnonymityValue(int value)
    {
        // Get current values from globals
        int currentAnonymityValue = globals.Get("current_anonymity_value").AsInt32();
        int maxAnonymityValue = globals.Get("max_anonimity_value").AsInt32();
        
        // Update anonymity value
        currentAnonymityValue += value;
        if (currentAnonymityValue > maxAnonymityValue)
        {
            currentAnonymityValue = maxAnonymityValue;
        }
        
        // Set the updated value back to globals
        globals.Set("current_anonymity_value", currentAnonymityValue);
        
        // Update the display text
        var textNode = GetChild(0);
        textNode.Call("set_text", $"Anonimity: {currentAnonymityValue}");
        
        // Emit the update signal
        EmitSignal(SignalName.AnonValueUpdate);
    }
}