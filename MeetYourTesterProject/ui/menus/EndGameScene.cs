using Godot;

public partial class EndGameScene : Node2D
{
	private Label titleLabel;
	private Label bodyLabel;
	private TextureRect background;
	
	// Access to autoloaded GDScript singleton
	private Node globals;

	public override void _Ready()
	{
		// Get reference to autoloaded GDScript singleton
		globals = GetNode<Node>("/root/Globals");
		
		// Get UI elements
		titleLabel = GetNode<Label>("title");
		bodyLabel = GetNode<Label>("body");
		background = GetNode<TextureRect>("bg");
		
		// Load and parse messages
		string messagesFilePath = globals.Get("messages_file_path").AsString();
		string messagesJson = FileAccess.GetFileAsString(messagesFilePath);
		
		var json = new Json();
		json.Parse(messagesJson);
		var messages = json.Data.AsGodotDictionary();
		
		// TODO use the actual value from the signal inside the match
		int endGameReason = globals.Get("end_game_reason").AsInt32();
		
		switch (endGameReason)
		{
			case 1:
				var sabotageData = messages["sabotage"].AsGodotDictionary();
				titleLabel.Text = sabotageData["title"].AsString();
				bodyLabel.Text = sabotageData["body"].AsString();
				background.Texture = GD.Load<Texture2D>("res://images/end-game/end-screen-sabotage.svg");
				break;
			case 2:
				var anonymityData = messages["anonimity"].AsGodotDictionary();
				titleLabel.Text = anonymityData["title"].AsString();
				bodyLabel.Text = anonymityData["body"].AsString();
				background.Texture = GD.Load<Texture2D>("res://images/end-game/end-screen-anonymity.svg");
				break;
			case 3:
				var deadlineData = messages["deadline"].AsGodotDictionary();
				titleLabel.Text = deadlineData["title"].AsString();
				bodyLabel.Text = deadlineData["body"].AsString();
				background.Texture = GD.Load<Texture2D>("res://images/end-game/end-screen-deadline.svg");
				break;
		}
	}

	public override void _Process(double delta)
	{
		// Empty - keeping for consistency with original
	}

	private void _on_restart_game_button_pressed()
	{
		ResetGlobals();
		GetTree().ChangeSceneToFile("res://ui/main_screen/main_game_scene.tscn");
	}

	// Reset all globals variables to initial state
	private void ResetGlobals()
	{
		globals.Set("gamePaused", false);
		globals.Set("gameSpeed", 1);
		globals.Set("gameTime", 0);
		
		// Get progress bar speeds and current difficulty level
		var progressBarSpeeds = globals.Get("progress_bar_possible_speeds").AsGodotArray();
		int currentDifficultyLevel = globals.Get("current_difficulty_level").AsInt32();
		globals.Set("progress_bar_speed", progressBarSpeeds[currentDifficultyLevel - 1]);
		
		var maxAnonymityValue = globals.Get("max_anonimity_value");
		globals.Set("current_anonymity_value", maxAnonymityValue);
		globals.Set("end_game_reason", new Variant());
	}

	private void _on_back_to_menu_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://ui/menus/main_menu.tscn");
	}
}
