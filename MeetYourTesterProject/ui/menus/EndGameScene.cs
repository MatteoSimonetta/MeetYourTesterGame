using Godot;
using System;
using Godot.Collections;

public partial class EndGameScene : Node2D
{
    private Label titleLabel;
    private Label bodyLabel;
    private TextureRect background;

    public override void _Ready()
    {
        titleLabel = GetNode<Label>("title");
        bodyLabel = GetNode<Label>("body");
        background = GetNode<TextureRect>("bg");

        var jsonString = FileAccess.GetFileAsString(Globals.Instance.MessagesFilePath);
        var json = new Json();
        json.Parse(jsonString);
        var messages = (Dictionary)json.Data;

        switch (Globals.Instance.EndGameReason)
        {
            case 1:
                titleLabel.Text = (string)((Dictionary)messages["sabotage"])["title"];
                bodyLabel.Text = (string)((Dictionary)messages["sabotage"])["body"];
                background.Texture = ResourceLoader.Load<Texture2D>("res://images/end-game/end-screen-sabotage.svg");
                break;

            case 2:
                titleLabel.Text = (string)((Dictionary)messages["anonimity"])["title"];
                bodyLabel.Text = (string)((Dictionary)messages["anonimity"])["body"];
                background.Texture = ResourceLoader.Load<Texture2D>("res://images/end-game/end-screen-anonymity.svg");
                break;

            case 3:
                titleLabel.Text = (string)((Dictionary)messages["deadline"])["title"];
                bodyLabel.Text = (string)((Dictionary)messages["deadline"])["body"];
                background.Texture = ResourceLoader.Load<Texture2D>("res://images/end-game/end-screen-deadline.svg");
                break;
        }
    }

    public override void _Process(double delta)
    {
    }

    private void OnRestartGameButtonPressed()
    {
        ResetGlobals();
        GetTree().ChangeSceneToFile("res://ui/main_screen/main_game_scene.tscn");
    }

    private void OnBackToMenuButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://ui/menus/main_menu.tscn");
    }

    private void ResetGlobals()
    {
        Globals.Instance.GamePaused = false;
        Globals.Instance.GameSpeed = 1;
        Globals.Instance.GameTime = 0;

        int difficultyIndex = (int)Globals.Instance.CurrentDifficultyLevel - 1;
        Globals.Instance.ProgressBarSpeed = Globals.Instance.ProgressBarPossibleSpeeds[difficultyIndex];

        Globals.Instance.CurrentAnonymityValue = Globals.Instance.MaxAnonimityValue;
        Globals.Instance.EndGameReason = null;
    }
}
