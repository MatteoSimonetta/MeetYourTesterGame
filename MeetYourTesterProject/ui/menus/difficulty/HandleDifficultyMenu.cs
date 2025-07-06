using Godot;

public partial class HandleDifficultyMenu : Node2D
{
	private TextureButton easyIcon;
	private TextureButton easyLabel;
	private TextureButton mediumIcon;
	private TextureButton mediumLabel;
	private TextureButton hardIcon;
	private TextureButton hardLabel;
	private TextureButton cancelIcon;
	private TextureButton cancelLabel;
	private Node globals;

	public override void _Ready()
	{
		// Get Global
		globals = GetNode<Node>("/root/Globals");
		
		// Get UI elements
		easyIcon = GetNode<TextureButton>("DiffBtns/EasyIcon");
		easyLabel = GetNode<TextureButton>("DiffBtns/CenterEasyLabel/EasyDiffBtn");
		mediumIcon = GetNode<TextureButton>("DiffBtns/MediumIcon");
		mediumLabel = GetNode<TextureButton>("DiffBtns/CenterMediumLabel/MediumDiffBtn");
		hardIcon = GetNode<TextureButton>("DiffBtns/HardIcon");
		hardLabel = GetNode<TextureButton>("DiffBtns/CenterHardLabel/HardDiffBtn");
		cancelIcon = GetNode<TextureButton>("DiffBtns/CancelIcon");
		cancelLabel = GetNode<TextureButton>("DiffBtns/CenterCancelLabel/CancelBtn");
	}

	public override void _Process(double delta)
	{
	}

	private void _on_diff_btn_mouse_entered(int difficultyLevel)
	{
		switch (difficultyLevel)
		{
			case 0:
				DebugPrint("Not ready yet (label) on hover enter");
				cancelIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-cancel-select.svg");
				cancelLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-cancel-select.svg");
				break;
			case 1:
				DebugPrint("Easy game (label) on hover enter");
				easyIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-easy-select.svg");
				easyLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-easy-select.svg");
				break;
			case 2:
				DebugPrint("Medium game (label) on hover enter");
				mediumIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-medium-select.svg");
				mediumLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-medium-select.svg");
				break;
			case 3:
				DebugPrint("Hard game (label) on hover enter");
				hardIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-hard-select.svg");
				hardLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-hard-select.svg");
				break;
		}
	}

	private void _on_diff_btn_mouse_exited(int difficultyLevel)
	{
		switch (difficultyLevel)
		{
			case 0:
				DebugPrint("Not ready yet (label) on hover exited");
				cancelIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-cancel.svg");
				cancelLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-cancel.svg");
				break;
			case 1:
				DebugPrint("Easy game (label) on hover exit");
				easyIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-easy.svg");
				easyLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-easy.svg");
				break;
			case 2:
				DebugPrint("Medium game (label) on hover exit");
				mediumIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-medium.svg");
				mediumLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-medium.svg");
				break;
			case 3:
				DebugPrint("Hard game (label) on hover exit");
				hardIcon.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-icon-hard.svg");
				hardLabel.TextureNormal = GD.Load<Texture2D>("res://images/start-scene/btn-label-hard.svg");
				break;
		}
	}

	private void _on_cancel_btn_pressed()
	{
		GetTree().ChangeSceneToFile("res://ui/menus/main_menu.tscn");
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
