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
	private Node globals;

	public override void _Ready()
	{
		globals = GetNode<Node>("/root/Globals");
		
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
		// Check bounds BEFORE accessing the array
		if (currentTutorialScreenIdx <= 0 && offset < 0)
		{
			// At first screen going backward - return to previous scene
			ReturnToPreviousScene();
			return;
		}
		
		if (currentTutorialScreenIdx >= jsonKeys.Count - 1 && offset > 0)
		{
			// At last screen going forward - return to previous scene
			ReturnToPreviousScene();
			return;
		}
		
		// Now it's safe to access the array
		string childName = jsonKeys[currentTutorialScreenIdx].AsString();
		
		// Hide the current scene
		FindChild(childName).Set("visible", false);
		
		currentTutorialScreenIdx += offset;
		
		// Bounds check after increment/decrement
		if (currentTutorialScreenIdx < 0)
		{
			currentTutorialScreenIdx = 0;
			ReturnToPreviousScene();
			return;
		}
		
		if (currentTutorialScreenIdx >= jsonKeys.Count)
		{
			ReturnToPreviousScene();
			return;
		}
		
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

	private void ReturnToPreviousScene()
	{
		// Get the stored previous scene node
		var previousSceneNode = globals.Get("previous_scene_node");
		
		if (previousSceneNode.VariantType != Variant.Type.Nil && previousSceneNode.AsGodotObject() != null)
		{
			// Show the previous scene
			if (previousSceneNode.AsGodotObject() is CanvasItem canvasItem)
			{
				canvasItem.Visible = true;
			}
			
			// If it was a game scene, unpause it
			var sceneNode = (Node)previousSceneNode;
			if (sceneNode.HasMethod("Resume"))
			{
				sceneNode.Call("Resume");
			}
			
			// Remove tutorial scene
			this.QueueFree();
		}
		else
		{
			// Fallback: use scene path if node reference is lost
			string previousScenePath = globals.Get("previous_scene_path").AsString();
			if (!string.IsNullOrEmpty(previousScenePath))
			{
				GetTree().ChangeSceneToFile(previousScenePath);
			}
			else
			{
				// Final fallback: go to main menu
				GetTree().ChangeSceneToFile("res://ui/menus/main_menu.tscn");
			}
		}
		
		// Clean up globals
		globals.Set("previous_scene_node", new Variant());
		globals.Set("previous_scene_path", "");
		// globals.Set("gamePaused", false);
	}

	private void _on_previous_pressed()
	{
		ChangeScene(-1);
	}

	private void _on_next_pressed()
	{
		ChangeScene(1);
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
