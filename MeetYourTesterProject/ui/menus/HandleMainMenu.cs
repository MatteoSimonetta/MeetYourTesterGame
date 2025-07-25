using Godot;

public partial class HandleMainMenu : Node
{
	private TextureButton startIcon;
	private TextureButton startLabel;
	private TextureButton quitIcon;
	private TextureButton quitLabel;
	private TextureButton tutorialLabel;
	private TextureButton tutorialIcon;
	private Control exitMenu;
	
	// Access to autoloaded GDScript singletons
	private Node globals;
	private Node utils;

	[Signal]
	public delegate void QuitSignalEventHandler();

	public override void _Ready()
	{
		// Get references to autoloaded GDScript singletons
		globals = GetNode<Node>("/root/Globals");
		utils = GetNode<Node>("/root/Utils");
		
		// Get UI elements
		startIcon = GetNode<TextureButton>("GridContainer/StartIcon");
		startLabel = GetNode<TextureButton>("GridContainer/CenterStartLabel/StartLabel");
		quitIcon = GetNode<TextureButton>("GridContainer/QuitIcon");
		quitLabel = GetNode<TextureButton>("GridContainer/CenterQuitLabel/QuitLabel");
		tutorialIcon = GetNode<TextureButton>("GridContainer/TutorialIcon");
		tutorialLabel = GetNode<TextureButton>("GridContainer/CenterTutorialLabel/TutorialLabel");
		exitMenu = GetNode<Control>("ExitMenuControl");
		
		exitMenu.Visible = false;
		
		// Connect signals
		QuitSignal += DisableEverything;
		var exitMenuNode = exitMenu.GetNode("exit_menu");
		exitMenuNode.Connect("ResumeFromQuitPrompt", new Callable(this, nameof(EnableEverything)));
		
		// Handle first-time startup configuration
		HandleFirstTimeStartup();
	}

	private void HandleFirstTimeStartup()
	{
		var config = new ConfigFile();
		var err = config.Load("./settings.cfg");
		
		if (err == Error.Ok)
		{
			var showPopup = config.GetValue("FirstStart", "show_popup");
			if (showPopup.AsBool())
			{
				ShowTutorialPopup();
				config.SetValue("FirstStart", "show_popup", false);
				config.Save("./settings.cfg");
			}
		}
		else
		{
			// First time that the user plays (file not present)
			config.SetValue("FirstStart", "show_popup", false);
			config.Save("./settings.cfg");
			ShowTutorialPopup();
		}
	}

	public override void _Process(double delta)
	{
	}

	private void ShowTutorialPopup()
	{
		GetNode<Control>("TutorialPopup").Visible = true;
	}

	private void DisableEverything()
	{
		// Call Utils.pause() from the GDScript autoload
		utils.Call("pause", GetNode("GridContainer"));
		exitMenu.Visible = true;
		
		// Call Utils.toggle_button_effect() from the GDScript autoload
		utils.Call("toggle_button_effect", startIcon);
		utils.Call("toggle_button_effect", startLabel);
		utils.Call("toggle_button_effect", quitLabel);
		utils.Call("toggle_button_effect", quitIcon);
		utils.Call("toggle_button_effect", tutorialLabel);
		utils.Call("toggle_button_effect", tutorialIcon);
	}

	private void EnableEverything()
	{
		// Call Utils.unpause() from the GDScript autoload
		utils.Call("unpause", GetNode("GridContainer"));
		exitMenu.Visible = false;
		
		// Call Utils.toggle_button_effect() from the GDScript autoload
		utils.Call("toggle_button_effect", startIcon);
		utils.Call("toggle_button_effect", startLabel);
		utils.Call("toggle_button_effect", quitLabel);
		utils.Call("toggle_button_effect", quitIcon);
		utils.Call("toggle_button_effect", tutorialLabel);
		utils.Call("toggle_button_effect", tutorialIcon);
	}

	// Start button handlers
	private void _on_start_game_label_pressed()
	{
		DebugPrint("Start Game Button (label) pressed");
		GetTree().ChangeSceneToFile("res://ui/menus/difficulty/diff_scene.tscn");
	}

	private void _on_start_label_mouse_entered()
	{
		DebugPrint("Start Game Button (label) on hover entered");
		if (!exitMenu.Visible)
		{
			startIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-start-select.svg");
		}
	}

	private void _on_start_label_mouse_exited()
	{
		DebugPrint("Start Game Button (label) on hover exited");
		startIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-start.svg");
	}

	private void _on_start_icon_mouse_entered()
	{
		DebugPrint("Start Game Button (icon) on hover entered");
		if (!exitMenu.Visible)
		{
			startLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-start-select.svg");
		}
	}

	private void _on_start_icon_mouse_exited()
	{
		DebugPrint("Start Game Button (icon) on hover exited");
		startLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-start.svg");
	}

	// Quit button handlers
	private void _on_quit_label_pressed()
	{
		DebugPrint("Quit Game Button (label) pressed");
		EmitSignal(SignalName.QuitSignal);
	}

	private void _on_quit_icon_mouse_entered()
	{
		DebugPrint("Quit Game Button (icon) on hover entered");
		if (!exitMenu.Visible)
		{
			quitLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-quit-select.svg");
		}
	}

	private void _on_quit_icon_mouse_exited()
	{
		DebugPrint("Quit Game Button (icon) on hover exited");
		quitLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-quit.svg");
	}

	private void _on_quit_label_mouse_entered()
	{
		DebugPrint("Quit Game Button (label) on hover entered");
		if (!exitMenu.Visible)
		{
			quitIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-exit-select.svg");
		}
	}

	private void _on_quit_label_mouse_exited()
	{
		DebugPrint("Quit Game Button (label) on hover exited");
		quitIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-exit.svg");
	}
	
	private void _on_tutorial_icon_mouse_entered()
	{
		DebugPrint("Tutorial Button (icon) on hover entered");
		if (!exitMenu.Visible)
		{
			tutorialLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-tutorial-select.svg");
		}
	}

	private void _on_tutorial_icon_mouse_exited()
	{
		DebugPrint("Tutorial Button (icon) on hover exited");
		tutorialLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-tutorial.svg");
	}

	private void _on_tutorial_label_mouse_entered()
	{
		DebugPrint("Tutorial Button (label) on hover entered");
		if (!exitMenu.Visible)
		{
			tutorialIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-tutorial-select.svg");
		}
	}

	private void _on_tutorial_label_mouse_exited()
	{
		DebugPrint("Tutorial Button (label) on hover exited");
		tutorialIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-tutorial.svg");
	}
	
	private void _on_start_tutorial_pressed()
	{
		DebugPrint("Start tutorial");

		// Store information about the current scene
		globals.Set("previous_scene_path", "res://ui/menus/main_menu.tscn");
		globals.Set("previous_scene_node", this.GetParent()); // Store the entire main menu scene

		// Load and instantiate tutorial scene
		var tutorialScene = GD.Load<PackedScene>("res://ui/menus/tutorial_scene.tscn");
		var tutorialInstance = tutorialScene.Instantiate();

		// Add tutorial scene to the same parent (scene root)
		GetParent().AddChild(tutorialInstance);

		// Hide the current scene instead of removing it (to preserve state)
		if (this.GetParent() is CanvasItem parentCanvas)
		{
			parentCanvas.Visible = false;
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
