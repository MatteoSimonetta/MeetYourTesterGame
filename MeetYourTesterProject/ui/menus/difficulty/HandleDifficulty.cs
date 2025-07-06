using Godot;

public partial class HandleDifficulty : TextureButton
{
    // Exported variables for setting difficulty in the Editor
    [ExportCategory("Difficulty")]
    [Export(PropertyHint.Enum, "Cancel:0,Easy:1,Medium:2,Hard:3")]
    public int DifficultyLevel { get; set; } = 0;

    [ExportCategory("Next scene path")]
    [Export]
    public string NextScenePath { get; set; } = "";

    // Access to autoloaded GDScript singletons
    private Node globals;
    private Node sceneManager;

    public override void _Ready()
    {
        // Get references to autoloaded GDScript singletons
        globals = GetNode<Node>("/root/Globals");
        sceneManager = GetNode<Node>("/root/SceneManager");
        
        // Connect the pressed signal
        Pressed += OnButtonPressed;
    }

    private void OnButtonPressed()
    {
        // Set the current difficulty level in Globals
        globals.Set("current_difficulty_level", DifficultyLevel);
        
        if (DifficultyLevel > 0)
        {
            ResetGlobals();
            LoadDeadlines();
        }
        
        // Change to the next scene
        if (!string.IsNullOrEmpty(NextScenePath))
        {
            sceneManager.Call("change_scene", NextScenePath);
        }
        else
        {
            DebugPrint("Next scene path is not set.");
        }
    }

    // Reset all globals variables to initial state
    private void ResetGlobals()
    {
        globals.Set("gamePaused", false);
        globals.Set("gameSpeed", 1);
        globals.Set("gameTime", 0);
        
        // Get progress_bar_possible_speeds array and set the speed based on difficulty
        var progressBarSpeeds = globals.Get("progress_bar_possible_speeds").AsGodotArray();
        globals.Set("progress_bar_speed", progressBarSpeeds[DifficultyLevel - 1]);
        
        var maxAnonymityValue = globals.Get("max_anonimity_value");
        globals.Set("current_anonymity_value", maxAnonymityValue);
        globals.Set("end_game_reason", Variant.CreateFrom((string)null));
    }

    // Load deadlines for the current difficulty
    private void LoadDeadlines()
    {
        string deadlinesFilePath = globals.Get("deadlines_file_path").AsString();
        string file = FileAccess.GetFileAsString(deadlinesFilePath);
        
        if (!string.IsNullOrEmpty(file))
        {
            var json = new Json();
            json.Parse(file);
            var parseJsonFile = json.Data.AsGodotDictionary();
            
            string difficultyKey = $"difficulty-{DifficultyLevel}";
            if (parseJsonFile.ContainsKey(difficultyKey))
            {
                globals.Set("deadlines", parseJsonFile[difficultyKey]);
            }
            else
            {
                globals.Set("deadlines", new Godot.Collections.Array());
            }
        }
        else
        {
            globals.Set("deadlines", new Godot.Collections.Array());
        }
    }

    private void DebugPrint(string msg)
    {
        // Access DEBUG_MODE from the Globals GDScript autoload
        bool debugMode = globals.Get("DEBUG_MODE").AsBool();
        if (debugMode)
        {
            GD.Print(msg);
        }
    }
}