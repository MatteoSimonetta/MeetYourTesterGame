using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;


[Tool]
public partial class HandleDifficulty : TextureButton
{
    [ExportCategory("Difficulty")]
    [Export(PropertyHint.Enum, "Cancel:0,Easy:1,Medium:2,Hard:3")]
    public int DifficultyLevel { get; set; } = 0;

    [ExportCategory("Next scene path")]
    [Export(PropertyHint.File, "*.tscn")]
    public string NextScenePath { get; set; } = "";

    public override void _Ready()
    {
        Pressed += OnButtonPressed;
    }

    private void OnButtonPressed()
    {
        Globals.Instance.CurrentDifficultyLevel = DifficultyLevel;

        if (DifficultyLevel > 0)
        {
            ResetGlobals();
            LoadDeadlines();
        }

        if (!string.IsNullOrEmpty(NextScenePath))
        {
            SceneManager.Instance.ChangeScene(NextScenePath);
        }
        else
        {
            DebugPrint("Next scene path is not set.");
        }
    }

    private void ResetGlobals()
    {
        Globals.Instance.GamePaused = false;
        Globals.Instance.GameSpeed = 1;
        Globals.Instance.GameTime = 0;

        Globals.Instance.ProgressBarSpeed = Globals.Instance.ProgressBarPossibleSpeeds[DifficultyLevel - 1];
        Globals.Instance.CurrentAnonymityValue = Globals.Instance.MaxAnonimityValue;
        Globals.Instance.EndGameReason = null;
    }

    private void LoadDeadlines()
    {
        string jsonString = FileAccess.GetFileAsString(Globals.Instance.DeadlinesFilePath);

        if (!string.IsNullOrEmpty(jsonString))
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                DifficultyDeadlines allDeadlines = JsonSerializer.Deserialize<DifficultyDeadlines>(jsonString, options);
                string key = $"difficulty-{DifficultyLevel}";

                if (allDeadlines != null && allDeadlines.ContainsKey(key))
                {
                    Globals.Instance.Deadlines = new Godot.Collections.Array();

                    foreach (var entry in allDeadlines[key])
                    {
                        foreach (var kvp in entry)
                        {
                            DeadlineData data = kvp.Value;
                            var dict = new Godot.Collections.Dictionary // this type is a Godot one and so is Variant
                            {
                                { "label", data.Label },
                                { "deadline_position_in_seconds", data.DeadlinePositionInSeconds }
                            };
                            Globals.Instance.Deadlines.Add(dict);
                        }
                    }
                }
                else
                {
                    Globals.Instance.Deadlines = new Godot.Collections.Array();
                    DebugPrint($"Key '{key}' not found in JSON.");
                }
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to parse deadlines JSON: {e.Message}");
                Globals.Instance.Deadlines = new Godot.Collections.Array();
            }
        }
        else
        {
            Globals.Instance.Deadlines = new Godot.Collections.Array();
            DebugPrint("Deadlines JSON file is empty or missing.");
        }
    }


    private void DebugPrint(string msg)
    {
        if (Globals.DEBUG_MODE)
            GD.Print(msg);
    }
}

internal class DifficultyDeadlines : Dictionary<string, List<Dictionary<string, DeadlineData>>> { }

internal class DeadlineData
{
    internal string Label { get; set; }

    [JsonPropertyName("deadline_position_in_seconds")]
    internal int DeadlinePositionInSeconds { get; set; }
}
