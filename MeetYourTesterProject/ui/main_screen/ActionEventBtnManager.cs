using Godot;

public partial class ActionEventBtnManager : TextureButton
{
    private Control exitMenu;
    
    [ExportCategory("Communication message")]
    [Export]
    public string Message { get; set; } = "Test message";
    
    private Texture2D backupDisableImage;
    private Texture2D backupHoverImage;
    private bool isActionEventGenerated = false;
    private Node timerChild = null;
    
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
        timerChild = GetChild(0).GetChild(0);
        
        // Connect the pressed signal
        Pressed += OnPressed;
    }

    public override void _Process(double delta)
    {
    }

    private void _on_timer_timeout()
    {
        if (!isActionEventGenerated)
        {
            StartSoundSpawnEvent();
            GenerateActionEvent();
        }
        else
        {
            RemoveActionEvent();
        }
    }

    private void OnPressed()
    {
        // To switch texture, we first save the disabled one, then replace it with the pressed one
        TextureDisabled = TexturePressed;
        Disabled = true;
        
        // Define the parameters to pass to the terminal
        string nodeName = Name;
        var parameters = new Godot.Collections.Dictionary
        {
            { "node_name", nodeName }
        };
        
        // Emit signal that this button has been pressed
        EmitSignal(SignalName.HexagonClicked, parameters);
        StartSoundClickedEvent();
    }

    // Functions to handle changes of state for the button
    private void GenerateActionEvent()
    {
        Disabled = false;
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