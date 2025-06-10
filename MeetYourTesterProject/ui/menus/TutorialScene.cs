using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class TutorialScene : Node2D
{
	private int currentTutorialScreenIdx = 0;
	private float speed = 16.0f;
	private int currentStep = 3;
	private string tutorialFilePath = "res://static-data/tutorial-content/tutorial.json";
	private Godot.Collections.Dictionary tutorialsContent = null;
	private string tutorialCurrentText = "";
	private int currentTutorialContentLength = 0;
	private Godot.Collections.Dictionary currentTutorialContent = null;
	private Godot.Collections.Array jsonKeys = null;

	public override void _Ready()
	{
		// Load and parse JSON file
		string jsonString = FileAccess.GetFileAsString(tutorialFilePath);
		var json = new Json();
		json.Parse(jsonString);
		tutorialsContent = json.Data.AsGodotDictionary();
		
		jsonKeys = new Godot.Collections.Array(tutorialsContent.Keys);
		string currentTutorialScreenKey = jsonKeys[currentTutorialScreenIdx].AsString();
		currentTutorialContentLength = tutorialsContent[currentTutorialScreenKey].AsGodotDictionary().Count;
		
		ChangeTutorialData();
	}

	public override void _PhysicsProcess(double delta)
	{
		var highlightSprite = GetNode<Sprite2D>("HighlightIcon");
		
		if (currentStep >= 0 && currentStep < 15)
		{
			highlightSprite.Position = new Vector2(highlightSprite.Position.X, highlightSprite.Position.Y + speed * (float)delta);
		}
		else
		{
			if (currentStep == -15)
			{
				currentStep = 15;
				return;
			}
			highlightSprite.Position = new Vector2(highlightSprite.Position.X, highlightSprite.Position.Y - speed * (float)delta);
		}

		currentStep -= 1;
	}

	private void ChangeScene(int offset = 1)
	{
		string childName = jsonKeys[currentTutorialScreenIdx].AsString();
		
		if (currentTutorialScreenIdx <= 0 && offset < 0)
		{
			return;
		}
		
		if (currentTutorialScreenIdx >= jsonKeys.Count - 1 && offset > 0)
		{
			GetTree().ChangeSceneToFile("res://ui/menus/main_menu.tscn");
			return;
		}
		
		// Hide the current scene
		FindChild(childName).Set("visible", false);
		
		currentTutorialScreenIdx += offset;
		childName = jsonKeys[currentTutorialScreenIdx].AsString();
		
		// Show new scene
		FindChild(childName).Set("visible", true);
		
		var nextButton = GetNode<TextureButton>("Popup/Next");
		if (currentTutorialScreenIdx + 1 == jsonKeys.Count)
		{
			nextButton.TextureNormal = GD.Load<Texture2D>("res://images/tutorial-popup/tutorial_finish_base.svg");
			nextButton.TextureHover = GD.Load<Texture2D>("res://images/tutorial-popup/tutorial_finish_hover.svg");
		}
		else
		{
			nextButton.TextureNormal = GD.Load<Texture2D>("res://images/tutorial-popup/next-button.svg");
			nextButton.TextureHover = GD.Load<Texture2D>("res://images/tutorial-popup/next-button-select.svg");
		}
		
		ChangeTutorialData();
	}

	private void _on_previous_pressed()
	{
		ChangeScene(-1);
	}

	private void _on_next_pressed()
	{
		ChangeScene();
	}

	private void ChangeTutorialData()
	{
		string currentTutorialScreenKey = jsonKeys[currentTutorialScreenIdx].AsString();
		currentTutorialContent = tutorialsContent[currentTutorialScreenKey].AsGodotDictionary();
		tutorialCurrentText = currentTutorialContent["text"].AsString();
		
		var highlightIcon = GetNode<Sprite2D>("HighlightIcon");
		var arrowPos = currentTutorialContent["arrow_pos"].AsGodotDictionary();
		highlightIcon.RotationDegrees = arrowPos["rot"].AsSingle();
		highlightIcon.Position = new Vector2(arrowPos["x"].AsSingle(), arrowPos["y"].AsSingle());
		
		var tutorialBody = GetNode<Label>("Popup/TutorialBody");
		tutorialBody.Text = tutorialCurrentText;
		
		var popup = GetNode<Control>("Popup");
		var popupData = currentTutorialContent["popup"].AsGodotDictionary();
		float scale = popupData["scale"].AsSingle();
		popup.Scale = new Vector2(scale, scale);
		
		var popupPos = popupData["pos"].AsGodotDictionary();
		popup.Position = new Vector2(popupPos["x"].AsSingle(), popupPos["y"].AsSingle());
		
		var progressionText = GetNode<RichTextLabel>("Popup/ProgressionText");
		progressionText.Text = $"{currentTutorialScreenIdx + 1}/{jsonKeys.Count}";
	}
}
