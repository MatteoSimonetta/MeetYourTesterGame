using Godot;
using System.Collections.Generic;

public partial class ProgressBarControl : Control
{
	private const string PROGRESS_BAR_DICTIONARY_KEY = "progress_bar";
	private const string PROGRESS_BAR_VALUE_DICTIONARY_KEY = "value";
	private const string PROGRESS_BAR_ZONE_DICTIONARY_KEY = "zone";
	
	private PackedScene zonesScene = GD.Load<PackedScene>("res://ui/main_screen/progress_bar_zones_scene.tscn");
	private PackedScene deadlineScene = GD.Load<PackedScene>("res://ui/main_screen/progress_bar_deadline_scene.tscn");
	
	private List<Godot.Collections.Dictionary> zonesQueue = new List<Godot.Collections.Dictionary>(); // FIFO queue to store created zones with their parameters
	private Godot.Collections.Dictionary zoneDictionary = new Godot.Collections.Dictionary();
	private string[] colors = { "red", "green" };
	private string[] sizes = { "sm", "md", "lg" };
	
	private Node terminal;
	
	// Access to autoloaded GDScript singletons
	private Node globals;
	private Node utils;

	[Signal]
	public delegate void DeadlineMissedEventHandler();
	
	[Signal]
	public delegate void ProgressBarLimitReachedEventHandler();
	
	[Signal]
	public delegate void LastDeadlineMissedEventHandler();

	public override void _Ready()
	{
		// Get references to autoloaded GDScript singletons
		globals = GetNode<Node>("/root/Globals");
		utils = GetNode<Node>("/root/Utils");
		
		// Get terminal reference
		terminal = GetNode("../Terminal/_terminal_mock/terminal_content");
		
		// Connect signals
		terminal.Connect("AnswerSignal", new Callable(this, nameof(ApplyProgressBarEffects)));
		
		// Initialize zone dictionary
		foreach (string color in colors)
		{
			zoneDictionary[color] = new Godot.Collections.Dictionary();
			foreach (string size in sizes)
			{
				var colorDict = zoneDictionary[color].AsGodotDictionary();
				colorDict[size] = GD.Load<Texture2D>($"res://images/main-game/progress-bar/{color}-zone-{size}.svg");
			}
		}
		
		CreateDeadlines();
		InitLastDeadlineLabel();
	}

	public override void _Process(double delta)
	{
		if (IsZonePresent())
		{
			int positionBar = GetCurrentPosition();
			
			// Check if bar is currently inside a spawned zone, then change its speed
			if (IsInsideZone(positionBar))
			{
				var gameProgressBar = GetNode("GameProgressBar");
				if (zonesQueue[0]["speed"].AsSingle() < 1)
				{
					gameProgressBar.Set("texture_progress", GD.Load<Texture2D>("res://images/main-game/progress-bar/yellow-bars-slower.svg"));
				}
				if (zonesQueue[0]["speed"].AsSingle() > 1)
				{
					gameProgressBar.Set("texture_progress", GD.Load<Texture2D>("res://images/main-game/progress-bar/yellow-bars-faster.svg"));
				}
			}
			
			// Check if bar has passed a spawned zone, then remove it
			if (positionBar > zonesQueue[0]["end_pos"].AsInt32())
			{
				RemoveZone();
			}
		}
	}

	private bool IsZonePresent()
	{
		return zonesQueue.Count > 0;
	}

	// If current progress is in the first zone
	private bool IsInsideZone(int positionBar)
	{
		return IsZonePresent() && 
			   zonesQueue[0]["start_pos"].AsInt32() < positionBar && 
			   positionBar < zonesQueue[0]["end_pos"].AsInt32();
	}

	private int GetCurrentPosition()
	{
		var gameProgressBar = GetNode("GameProgressBar");
		float value = gameProgressBar.Get("value").AsSingle();
		var size = gameProgressBar.Get("size").AsVector2();
		return GetPixelFromPercent(value, (int)size.X);
	}

	private int GetPixelFromPercent(float percent, int total)
	{
		return (int)(percent * total / 100);
	}

	// Called by the game timer each cycle, increment bar progress by default and apply zone modifier
	public void AutoIncrement()
	{
		var gameProgressBar = GetNode("GameProgressBar");
		var progressBarSpeedDbg = GetNode("ProgressBarSpeedDbg");
		
		float currentValue = gameProgressBar.Get("value").AsSingle();
		progressBarSpeedDbg.Call("set_text", currentValue.ToString());
		
		// If bar has reached the end, print GAME OVER
		float progressBarSpeed = globals.Get("progress_bar_speed").AsSingle();
		
		if (IsInsideZone(GetCurrentPosition()))
		{
			float zoneSpeed = zonesQueue[0]["speed"].AsSingle();
			gameProgressBar.Set("value", currentValue + progressBarSpeed * zoneSpeed);
		}
		else
		{
			gameProgressBar.Set("value", currentValue + progressBarSpeed);
		}
		
		CheckProgressBarLimitReached();
		DecreaseDeadlinesTimers();
	}

	private void CheckProgressBarLimitReached()
	{
		var gameProgressBar = GetNode("GameProgressBar");
		float value = gameProgressBar.Get("value").AsSingle();
		float maxValue = gameProgressBar.Get("max_value").AsSingle();
		
		if (value >= maxValue)
		{
			// TODO add an actual end game notification, stopping the game timer
			GD.Print("GAME OVER: progress bar has reached 100%");
			EmitSignal(SignalName.ProgressBarLimitReached);
		}
	}

	// Get effects from answer and apply them (moving progress, creating zone)
	private void ApplyProgressBarEffects(Godot.Collections.Dictionary selectedAnswer)
	{
		GD.Print("apply_progress_bar_effects");
		if (selectedAnswer.ContainsKey(PROGRESS_BAR_DICTIONARY_KEY))
		{
			var effect = selectedAnswer[PROGRESS_BAR_DICTIONARY_KEY].AsGodotDictionary();
			
			if (effect.ContainsKey(PROGRESS_BAR_VALUE_DICTIONARY_KEY))
			{
				// Godot handles under the hood the check for progress bar boundaries. If you add 1000 with a max value of 100 it will be 100.
				var gameProgressBar = GetNode("GameProgressBar");
				float currentValue = gameProgressBar.Get("value").AsSingle();
				float addValue = effect[PROGRESS_BAR_VALUE_DICTIONARY_KEY].AsSingle();
				gameProgressBar.Set("value", currentValue + addValue);
				CheckProgressBarLimitReached();
			}
			
			if (effect.ContainsKey(PROGRESS_BAR_ZONE_DICTIONARY_KEY))
			{
				CreateZone(effect[PROGRESS_BAR_ZONE_DICTIONARY_KEY].AsGodotDictionary());
			}
		}
	}

	private void InitLastDeadlineLabel()
	{
		// Last label is max value of the progress bar / current step size
		var gameProgressBar = GetNode("GameProgressBar");
		var finalDeadlineLabel = GetNode("FinalDeadlineLabel");
		
		float maxValue = gameProgressBar.Get("max_value").AsSingle();
		float progressBarSpeed = globals.Get("progress_bar_speed").AsSingle();
		float timeValue = maxValue / progressBarSpeed;
		
		string timeString = utils.Call("float_to_time", timeValue).AsString();
		finalDeadlineLabel.Call("set_text", timeString);
	}

	private void CreateDeadlines()
	{
		var deadlines = globals.Get("deadlines").AsGodotArray();
		var gameProgressBar = GetNode("GameProgressBar");
		var deadlinesContainer = GetNode("DeadlinesContainer");
		
		for (int i = 0; i < deadlines.Count; i++)
		{
			var newDeadlineScene = deadlineScene.Instantiate();
			string currentKey = $"deadline_{i}";
			var deadlineData = deadlines[i].AsGodotDictionary()[currentKey].AsGodotDictionary();
			
			int deadlinePosition = deadlineData["deadline_position_in_seconds"].AsInt32();
			string timeString = utils.Call("float_to_time", (float)deadlinePosition).AsString();
			
			newDeadlineScene.GetChild(0).Call("set_text", timeString);
			
			float progressBarSpeed = globals.Get("progress_bar_speed").AsSingle();
			var gameProgressBarSize = gameProgressBar.Get("size").AsVector2();
			float positionX = (deadlinePosition * progressBarSpeed / 100 * gameProgressBarSize.X) - newDeadlineScene.GetChild(1).Get("size").AsVector2().X;
			
			newDeadlineScene.Set("position", new Vector2(positionX, newDeadlineScene.Get("position").AsVector2().Y));
			deadlinesContainer.Call("add_child", newDeadlineScene);
		}
	}

	private void AddChargeToSabotage()
	{
		GD.Print("deadline_missed signal emitted");
		EmitSignal(SignalName.DeadlineMissed);
	}

	private void MissedLastDeadline()
	{
		GD.Print("missed last deadline");
		EmitSignal(SignalName.LastDeadlineMissed);
	}

	private bool IsDeadlineReached(int deadlineIndex)
	{
		var gameProgressBar = GetNode("GameProgressBar");
		var progressFrame = GetNode("ProgressFrame");
		var deadlinesContainer = GetNode("DeadlinesContainer");
		
		float progressFrameBorder = gameProgressBar.Get("position").AsVector2().X - progressFrame.Get("position").AsVector2().X;
		float progressValue = gameProgressBar.Get("value").AsSingle();
		float maxValue = gameProgressBar.Get("max_value").AsSingle();
		var progressBarSize = gameProgressBar.Get("size").AsVector2();
		
		var deadlineChild = deadlinesContainer.GetChild(deadlineIndex);
		var deadlinePosition = deadlineChild.Get("position").AsVector2();
		var deadlineSize = deadlineChild.Get("size").AsVector2();
		var containerPosition = deadlinesContainer.Get("position").AsVector2();
		var framePosition = progressFrame.Get("position").AsVector2();
		
		return (progressValue / maxValue) * progressBarSize.X >= 
			   deadlinePosition.X + deadlineSize.X + containerPosition.X + framePosition.X + progressFrameBorder;
	}

	private void DecreaseDeadlinesTimers()
	{
		var finalDeadlineLabel = GetNode("FinalDeadlineLabel");
		if (finalDeadlineLabel.Get("text").AsString() == "00:00")
		{
			MissedLastDeadline();
			return;
		}

		int gameTime = globals.Get("gameTime").AsInt32();
		var gameProgressBar = GetNode("GameProgressBar");
		float maxValue = gameProgressBar.Get("max_value").AsSingle();
		float progressBarSpeed = globals.Get("progress_bar_speed").AsSingle();
		
		GD.Print(gameTime);
		float remainingTime = (maxValue / progressBarSpeed) - gameTime;
		GD.Print(remainingTime);
		
		string timeString = utils.Call("float_to_time", remainingTime).AsString();
		finalDeadlineLabel.Call("set_text", timeString);

		var deadlinesContainer = GetNode("DeadlinesContainer");
		var children = deadlinesContainer.GetChildren();
		
		for (int i = 0; i < children.Count; i++)
		{
			var deadlineChild = children[i];
			var deadlineLabel = deadlineChild.GetChild(0);
			var deadlineTexture = deadlineChild.GetChild(1);
			
			string labelText = deadlineLabel.Get("text").AsString();
			float currentTime = utils.Call("time_to_seconds", labelText).AsSingle();
			float newTimer = currentTime - 1;

			// Check if the texture is already changed
			string texturePath = deadlineTexture.Get("texture").As<Texture2D>().ResourcePath;
			if (texturePath.Contains("missed") || texturePath.Contains("reached"))
			{
				continue;
			}

			if (newTimer <= 0)
			{
				newTimer = 0;
				// Check if the current position of the progress bar is less than the position of the next deadline
				if (!IsDeadlineReached(i))
				{
					deadlineTexture.Set("texture", GD.Load<Texture2D>("res://images/main-game/progress-bar/deadline-missed.svg"));
					AddChargeToSabotage();
				}
				else
				{
					deadlineTexture.Set("texture", GD.Load<Texture2D>("res://images/main-game/progress-bar/deadline-reached.svg"));
				}
			}
			else
			{
				if (IsDeadlineReached(i))
				{
					deadlineTexture.Set("texture", GD.Load<Texture2D>("res://images/main-game/progress-bar/deadline-reached.svg"));
				}
			}

			string newTimeString = utils.Call("float_to_time", newTimer).AsString();
			deadlineLabel.Call("set_text", newTimeString);
		}
	}

	// Create a new zone and push it at the end of the queue
	private void CreateZone(Godot.Collections.Dictionary zoneEffects)
	{
		var newZoneNode = CreateNewZoneNode();
		SetZoneTexture(newZoneNode, zoneEffects);
		var newZone = SetNewZoneProperties(zoneEffects, newZoneNode);
		AddZoneToQueueAndContainer(newZone, newZoneNode);
	}

	// Create a new zone scene and node
	private TextureRect CreateNewZoneNode()
	{
		var newZoneScene = zonesScene.Instantiate();
		var newZoneNode = newZoneScene.GetChild<TextureRect>(0);
		newZoneScene.RemoveChild(newZoneNode);
		return newZoneNode;
	}

	// Set zone texture based on length (sm,md,lg)
	private void SetZoneTexture(TextureRect newZoneNode, Godot.Collections.Dictionary zoneEffects)
	{
		int zoneLength = zoneEffects.GetValueOrDefault("length", globals.Get("progress_bar_zone_length").AsGodotDictionary()["SMALL"]).AsInt32();
		float zoneSpeedup = zoneEffects.GetValueOrDefault("speedValue", 1).AsSingle();
		string zoneColor = zoneSpeedup < 1 ? "red" : "green";
		
		var progressBarZoneLength = globals.Get("progress_bar_zone_length").AsGodotDictionary();
		
		if (zoneLength == progressBarZoneLength["SMALL"].AsInt32())
		{
			newZoneNode.Texture = zoneDictionary[zoneColor].AsGodotDictionary()["sm"].As<Texture2D>();
		}
		else if (zoneLength == progressBarZoneLength["MEDIUM"].AsInt32())
		{
			newZoneNode.Texture = zoneDictionary[zoneColor].AsGodotDictionary()["md"].As<Texture2D>();
		}
		else if (zoneLength == progressBarZoneLength["LARGE"].AsInt32())
		{
			newZoneNode.Texture = zoneDictionary[zoneColor].AsGodotDictionary()["lg"].As<Texture2D>();
		}
		else
		{
			GD.Print($"Unrecognized zone length: {zoneLength}");
		}
	}

	// Set new zone's properties
	private Godot.Collections.Dictionary SetNewZoneProperties(Godot.Collections.Dictionary zoneEffects, TextureRect newZoneNode)
	{
		var newZone = new Godot.Collections.Dictionary();
		int zoneOffset = zoneEffects.GetValueOrDefault("offset", 0).AsInt32();
		newZone["speed"] = zoneEffects.GetValueOrDefault("speedValue", 1).AsSingle();
		
		int startPos = zoneOffset + (IsZonePresent() ? zonesQueue[zonesQueue.Count - 1]["end_pos"].AsInt32() : GetCurrentPosition());
		newZone["start_pos"] = startPos;
		newZone["end_pos"] = startPos + newZoneNode.Texture.GetWidth();
		newZoneNode.Set("offset_left", startPos);
		
		return newZone;
	}

	// Add new zone to queue and ZonesContainer
	private void AddZoneToQueueAndContainer(Godot.Collections.Dictionary newZone, TextureRect newZoneNode)
	{
		var zonesContainer = GetNode("ZonesContainer");
		var containerSize = zonesContainer.Get("size").AsVector2();
		
		if (newZone["end_pos"].AsInt32() <= containerSize.X)
		{
			zonesContainer.Call("add_child", newZoneNode);
			zonesQueue.Add(newZone);
		}
	}

	private void RemoveZone()
	{
		var zonesContainer = GetNode("ZonesContainer");
		var zone = zonesContainer.GetChild(0);
		zonesContainer.Call("remove_child", zone);
		zonesQueue.RemoveAt(0);
		
		var gameProgressBar = GetNode("GameProgressBar");
		gameProgressBar.Set("texture_progress", GD.Load<Texture2D>("res://images/main-game/progress-bar/yellow-bars-neutral.svg"));
	}
}
