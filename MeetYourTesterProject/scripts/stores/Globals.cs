using Godot;
using System;
using System.Collections.Generic;

public partial class Globals : Node
{
    // Global debug flag.
    public static bool DEBUG_MODE = true;

    // Game state variables.
    public bool gamePaused = false;
    public int gameSpeed = 1;
    public int gameTime = 0;

    // Uninitialized global variable.
    public object current_difficulty_level;

    // File paths.
    public string deadlines_file_path = "res://deadlines/Deadlines.json";
    public Godot.Collections.Array deadlines = new Godot.Collections.Array();

    public string questions_dir_path = "res://questions";
    public string questions_file_path = "res://questions/example_question.json";
    public string messages_file_path = "res://messages/end_game.json";
    public string questions_test_file_path = "res://questions/example_question_test.json";
    public Godot.Collections.Dictionary questions = new Godot.Collections.Dictionary();

    // Terminal and answer history.
    public string terminalHistory = "";
    public Godot.Collections.Dictionary currentAnswer = new Godot.Collections.Dictionary();

    // Anonymity values.
    public int current_anonymity_value = 100;
    public int anonymity_value_alert_threshold = 4;
    public int max_anonimity_value = 100;

    // Enum for progress bar zone length.
    public enum ProgressBarZoneLength
    {
        SMALL = 1,
        MEDIUM = 2,
        LARGE = 3,
    }

    // Progress bar speeds.
    public Godot.Collections.Array progress_bar_possible_speeds = new Godot.Collections.Array { 0.3, 0.6, 1.0 };
    public double progress_bar_speed = 0.3; // default to first element

    // Timers for action events.
    public int randomTimerForActionEventInactivity = 10; // originally 10, commented value 75 in GDScript
    public int randomTimerForActionEventAcceptance = 10;

    // Example of a constant definition. Since the GDScript cut off, define a placeholder.
    public const string ANONYMITY_BAR = "ANONYMITY_BAR";
}
