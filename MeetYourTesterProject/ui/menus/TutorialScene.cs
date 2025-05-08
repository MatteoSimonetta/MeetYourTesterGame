using Godot;
using System;
using Godot.Collections;

public partial class TutorialScene : Node2D
{
    private int currentTutorialScreenIdx = 0;
    private int speed = 16;
    private int currentStep = 3;
    private string tutorialFilePath = "res://static-data/tutorial-content/tutorial.json";
    private Dictionary tutorialsContent;
    private string tutorialCurrentText = "";
    private int currentTutorialContentLength = 0;
    private Dictionary currentTutorialContent;
    private Godot.Collections.Array jsonKeys;

    public override void _Ready()
    {
        var jsonString = FileAccess.GetFileAsString(tutorialFilePath);
        var json = new Json();
        json.Parse(jsonString);
        tutorialsContent = (Dictionary)json.Data;

        jsonKeys = new Godot.Collections.Array(tutorialsContent.Keys);

        string currentKey = (string)jsonKeys[currentTutorialScreenIdx];
        currentTutorialContentLength = ((Dictionary)tutorialsContent[currentKey]).Count;

        ChangeTutorialData();
    }

    public override void _PhysicsProcess(double delta)
    {
        var highlightSprite = GetNode<Sprite2D>("HighlightIcon");

        if (currentStep is >= 0 and <= 15)
        {
            highlightSprite.Position += new Vector2(0, speed * (float)delta);
        }
        else
        {
            if (currentStep == -15)
            {
                currentStep = 15;
                return;
            }
            highlightSprite.Position -= new Vector2(0, speed * (float)delta);
        }

        currentStep--;
    }

    private void ChangeScene(int offset = 1)
    {
        string childName = (string)jsonKeys[currentTutorialScreenIdx];

        if (currentTutorialScreenIdx <= 0 && offset < 0)
            return;

        if (currentTutorialScreenIdx >= jsonKeys.Count - 1 && offset > 0)
        {
            GetTree().ChangeSceneToFile("res://ui/menus/main_menu.tscn");
            return;
        }

        GetNode<CanvasItem>(childName).Visible = false;
        currentTutorialScreenIdx += offset;
        childName = (string)jsonKeys[currentTutorialScreenIdx];
        GetNode<CanvasItem>(childName).Visible = true;

        var nextBtn = GetNode<TextureButton>("Popup/Next");
        if (currentTutorialScreenIdx + 1 == jsonKeys.Count)
        {
            nextBtn.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/tutorial-popup/tutorial_finish_base.svg");
            nextBtn.TextureHover = ResourceLoader.Load<Texture2D>("res://images/tutorial-popup/tutorial_finish_hover.svg");
        }
        else
        {
            nextBtn.TextureNormal = ResourceLoader.Load<Texture2D>("res://images/tutorial-popup/next-button.svg");
            nextBtn.TextureHover = ResourceLoader.Load<Texture2D>("res://images/tutorial-popup/next-button-select.svg");
        }

        ChangeTutorialData();
    }

    private void OnPreviousPressed() => ChangeScene(-1);
    private void OnNextPressed() => ChangeScene();

    private void ChangeTutorialData()
    {
        string currentKey = (string)jsonKeys[currentTutorialScreenIdx];
        currentTutorialContent = (Dictionary)tutorialsContent[currentKey];

        tutorialCurrentText = (string)currentTutorialContent["text"];

        var arrowPos = (Dictionary)currentTutorialContent["arrow_pos"];
        var popupData = (Dictionary)currentTutorialContent["popup"];

        var highlight = GetNode<Sprite2D>("HighlightIcon");
        highlight.RotationDegrees = (float)arrowPos["rot"];
        highlight.Position = new Vector2((float)arrowPos["x"], (float)arrowPos["y"]);

        GetNode<RichTextLabel>("Popup/TutorialBody").Text = tutorialCurrentText;

        var popup = GetNode<Control>("Popup");
        float scale = (float)popupData["scale"];
        popup.Scale = new Vector2(scale, scale);
        popup.Position = new Vector2((float)popupData["pos"].As<Dictionary>()["x"], (float)popupData["pos"].As<Dictionary>()["y"]);

        GetNode<RichTextLabel>("Popup/ProgressionText").Text = $"{currentTutorialScreenIdx + 1}/{jsonKeys.Count}";
    }
}
