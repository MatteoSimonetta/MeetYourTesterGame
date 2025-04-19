using Godot;
using System;

public partial class Globals : Node
{
    public static Globals Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        ProgressBarSpeed = ProgressBarPossibleSpeeds[0];
    }

    public static readonly bool DEBUG_MODE = true;

    [Export] private bool _gamePaused = false;
    public bool GamePaused { get => _gamePaused; set => _gamePaused = value; }

    [Export] private float _gameSpeed = 1f;
    public float GameSpeed { get => _gameSpeed; set => _gameSpeed = value; }

    [Export] private float _gameTime = 0f;
    public float GameTime { get => _gameTime; set => _gameTime = value; }

    // Variant Godot way to store a variable with possible different type. I need to check if it is possible to remove it
    public Variant CurrentDifficultyLevel { get; set; }

    [Export] private string _deadlinesFilePath = "res://deadlines/Deadlines.json";
    public string DeadlinesFilePath { get => _deadlinesFilePath; set => _deadlinesFilePath = value; }
    
    public Godot.Collections.Array Deadlines { get; set; } = new Godot.Collections.Array();
    
    [Export] private string _questionsDirPath = "res://questions";
    public string QuestionsDirPath { get => _questionsDirPath; set => _questionsDirPath = value; }
    
    [Export] private string _questionsFilePath = "res://questions/example_question.json";
    public string QuestionsFilePath { get => _questionsFilePath; set => _questionsFilePath = value; }
    
    [Export] private string _messagesFilePath = "res://messages/end_game.json";
    public string MessagesFilePath { get => _messagesFilePath; set => _messagesFilePath = value; }

    [Export] private string _questionsTestFilePath = "res://questions/example_question_test.json";
    public string QuestionsTestFilePath { get => _questionsTestFilePath; set => _questionsTestFilePath = value; }
    
    public Godot.Collections.Dictionary Questions { get; set; } = new Godot.Collections.Dictionary();
    
    [Export] private string _terminalHistory = "";
    public string TerminalHistory { get => _terminalHistory; set => _terminalHistory = value; }
    
    public Godot.Collections.Dictionary CurrentAnswer { get; set; } = new Godot.Collections.Dictionary();
    
    [Export] private int _currentAnonymityValue = 100;
    public int CurrentAnonymityValue { get => _currentAnonymityValue; set => _currentAnonymityValue = value; }
    
    [Export] private int _anonymityValueAlertThreshold = 4;
    public int AnonymityValueAlertThreshold { get => _anonymityValueAlertThreshold; set => _anonymityValueAlertThreshold = value; }
    
    [Export] private int _maxAnonimityValue = 100;
    public int MaxAnonimityValue { get => _maxAnonimityValue; set => _maxAnonimityValue = value; }
    
    public enum ProgressBarZoneLength
    {
        SMALL = 1,
        MEDIUM = 2,
        LARGE = 3
    }
    
    public float[] ProgressBarPossibleSpeeds { get; } = new float[] { 0.3f, 0.6f, 1f };
    
    private float _progressBarSpeed;
    
    public float ProgressBarSpeed { get => _progressBarSpeed; set => _progressBarSpeed = value; }

    [Export] private int _randomTimerForActionEventInactivity = 10;
    public int RandomTimerForActionEventInactivity { 
        get => _randomTimerForActionEventInactivity; 
        set => _randomTimerForActionEventInactivity = value; 
    }
    
    [Export] private int _randomTimerForActionEventAcceptance = 10;
    public int RandomTimerForActionEventAcceptance { 
        get => _randomTimerForActionEventAcceptance; 
        set => _randomTimerForActionEventAcceptance = value; 
    }
    
    public static readonly string ANONYMITY_BAR_DICTIONARY_KEY = "anon_bar";
    
    public static readonly int ANON_VALUE_SABOTAGE_TRIGGER = 50;
    
    public static readonly int SABOTAGE_ANON_DECREASE_VALUE = 20;
    
    [Export] private int _decreaseFinalDeadlineAmount = 10;
    public int DecreaseFinalDeadlineAmount { 
        get => _decreaseFinalDeadlineAmount; 
        set => _decreaseFinalDeadlineAmount = value; 
    
    }
    public int? EndGameReason { get; set; }
    
    [Export] private float _bgMusicVolume = 0f;
    public float BgMusicVolume { get => _bgMusicVolume; set => _bgMusicVolume = value; }

    [Export] private float _soundFxVolume = 0f;
    public float SoundFxVolume { get => _soundFxVolume; set => _soundFxVolume = value; }
}
