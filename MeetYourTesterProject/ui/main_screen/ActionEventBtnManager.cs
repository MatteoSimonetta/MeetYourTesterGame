using Godot;

public partial class ActionEventBtnManager : TextureButton
{
    private Control exitMenu;
    
    [ExportCategory("Communication message")]
    [Export]
    public string Message { get; set; } = "Test message";
    
    private Texture2D backupDisableImage;
    private Texture2D backupHoverImage;
    private Texture2D backupNormalImage; // Store original normal texture
    private bool isActionEventGenerated = false;
    private Node timerChild = null;
    private bool isTemporarilyDisabled = false; // New flag for temporary disable
    
    // Access to autoloaded GDScript singleton
    private Node globals;

    [Signal]
    public delegate void HexagonClickedEventHandler(Godot.Collections.Dictionary parameters);

    public override void _Ready()
    {
        // Get reference to autoloaded GDScript singleton
        globals = GetNode<Node>("/root/Globals");
        
        var random = new RandomNumberGenerator();
        random.Randomize();
        
        backupDisableImage = TextureDisabled;
        backupHoverImage = TextureHover;
        backupNormalImage = TextureNormal; // Store original normal texture
        timerChild = GetChild(0).GetChild(0);
        
        // Connect the pressed signal
        Pressed += OnPressed;
        
        // Connect to terminal's answer signal to re-enable hexagons
        var terminal = GetNode("../../Terminal/_terminal_mock/terminal_content");
        if (terminal != null)
        {
            terminal.Connect("AnswerSignal", new Callable(this, nameof(OnAnswerGiven)));
        }
    }

    public override void _Process(double delta)
    {
    }

    private void _on_timer_timeout()
    {
        if (!isActionEventGenerated && !isTemporarilyDisabled)
        {
            StartSoundSpawnEvent();
            GenerateActionEvent();
        }
        else if (isActionEventGenerated && !isTemporarilyDisabled)
        {
            RemoveActionEvent();
        }
    }

    private void OnPressed()
    {
        // Only allow pressing if not temporarily disabled
        if (isTemporarilyDisabled) return;
        
        // To switch texture, we first save the disabled one, then replace it with the pressed one
        TextureDisabled = TexturePressed;
        Disabled = true;
        
        // Define the parameters to pass to the terminal
        string nodeName = Name;
        var parameters = new Godot.Collections.Dictionary
        {
            { "node_name", nodeName }
        };
        
        // Disable all other hexagons
        DisableAllOtherHexagons();
        
        // Emit signal that this button has been pressed
        EmitSignal(SignalName.HexagonClicked, parameters);
        StartSoundClickedEvent();
    }

    private void DisableAllOtherHexagons()
    {
        // Get all hexagon nodes (adjust paths according to your scene structure)
        var hexagonNodes = new string[] 
        { 
            "Database", 
            "Delivery", 
            "Business_Logic", 
            "Backend", 
            "UI_UX" 
        };
        
        foreach (string hexagonName in hexagonNodes)
        {
            var hexagonNode = GetNode("../" + hexagonName);
            if (hexagonNode != null && hexagonNode != this.GetParent())
            {
                hexagonNode.Call("SetTemporaryDisable", true);
            }
        }
    }

    private void OnAnswerGiven(Godot.Collections.Dictionary answerData)
    {
        // Re-enable all hexagons when an answer is given
        EnableAllHexagons();
    }

    private void EnableAllHexagons()
    {
        // Get all hexagon nodes
        var hexagonNodes = new string[] 
        { 
            "Database", 
            "Delivery", 
            "Business_Logic", 
            "Backend", 
            "UI_UX" 
        };
        
        foreach (string hexagonName in hexagonNodes)
        {
            var hexagonNode = GetNode("../" + hexagonName);
            if (hexagonNode != null)
            {
                hexagonNode.Call("SetTemporaryDisable", false);
            }
        }
    }

    // Public method to be called by other hexagons
    public void SetTemporaryDisable(bool disable)
    {
        isTemporarilyDisabled = disable;
        
        if (disable)
        {
            // If this hexagon has an active action event, disable it temporarily
            if (isActionEventGenerated)
            {
                Disabled = true;
                // Optionally change texture to show it's temporarily disabled
                TextureNormal = TextureDisabled;
            }
        }
        else
        {
            // Re-enable if it had an action event
            if (isActionEventGenerated)
            {
                Disabled = false;
                // Restore the original normal texture (the white/clickable texture)
                // We need to restore the texture that was set when the action event was generated
                // This should be the original TextureNormal before any modifications
                RestoreOriginalTextures();
            }
        }
    }

    private void RestoreOriginalTextures()
    {
        // Restore textures to their original state when action event is active
        if (isActionEventGenerated)
        {
            // Restore the white/clickable texture that indicates an active action event
            TextureNormal = backupNormalImage; // This should be the white texture
            TextureHover = backupHoverImage;
        }
    }

    // Functions to handle changes of state for the button
    private void GenerateActionEvent()
    {
        if (!isTemporarilyDisabled)
        {
            Disabled = false;
        }
        isActionEventGenerated = true;
        
        if (timerChild != null)
        {
            var random = new RandomNumberGenerator();
            random.Randomize();
            int randomAcceptanceTime = globals.Get("randomTimerForActionEventAcceptance").AsInt32();
            timerChild.Set("wait_time", random.RandiRange(0, randomAcceptanceTime));
        }
    }

    public void RemoveActionEvent()
    {
        TextureDisabled = backupDisableImage;
        TextureNormal = backupNormalImage; // Restore original white texture
        Disabled = true;
        isActionEventGenerated = false;
        
        if (timerChild != null)
        {
            timerChild.Call("stop");
            
            var random = new RandomNumberGenerator();
            random.Randomize();
            int randomInactivityTime = globals.Get("randomTimerForActionEventInactivity").AsInt32();
            timerChild.Set("wait_time", random.RandiRange(0, randomInactivityTime));
            timerChild.Call("start");
        }
    }

    private void StartSoundSpawnEvent()
    {
        GetNode("../HexagonActivatedSfx").Call("play");
    }

    private void StartSoundClickedEvent()
    {
        GetNode("../HexagonClickedSfx").Call("play");
    }

    private void HandleGameExit(bool checkGameQuit)
    {
        if (checkGameQuit)
        {
            if (Disabled && isActionEventGenerated)
            {
                TextureHover = TexturePressed;
            }
            if (Disabled && !isActionEventGenerated)
            {
                TextureHover = TextureDisabled;
            }
            if (!Disabled)
            {
                TextureHover = TextureNormal;
            }
        }
        else
        {
            TextureHover = backupHoverImage;
        }
    }
}