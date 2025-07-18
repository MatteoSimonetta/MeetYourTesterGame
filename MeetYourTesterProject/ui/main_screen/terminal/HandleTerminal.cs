using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class HandleTerminal : RichTextLabel
{
	private Tween tween;
	private RandomNumberGenerator rng = new RandomNumberGenerator();
	private List<object[]> queue = new List<object[]>();
	private string terminalHistory = "";
	private RegEx regexEngine = new RegEx();
	
	// For hover highlighting
	private string currentHoveredMeta = "";
	private string currentQuestionText = "";
	
	// Access to autoloaded GDScript singleton
	private Node globals;

	[Signal]
	public delegate void AnswerSignalEventHandler(Godot.Collections.Dictionary answerTarget);

	public override void _Ready()
	{
		// Get reference to autoloaded GDScript singleton
		globals = GetNode<Node>("/root/Globals");
		
		tween = GetTree().CreateTween();
		Clear();
		MetaUnderlined = false;
		ScrollFollowing = true;
		
		// Connect the meta clicked signal
		MetaClicked += HandleMetaClicked;
		
		// Connect hover signals for highlighting
		MetaHoverStarted += OnMetaHoverStarted;
		MetaHoverEnded += OnMetaHoverEnded;
	}

	private void OnMetaHoverStarted(Variant meta)
	{
		currentHoveredMeta = meta.AsString();
		RefreshCurrentQuestionWithHover();
	}

	private void OnMetaHoverEnded(Variant meta)
	{
		currentHoveredMeta = "";
		RefreshCurrentQuestionWithHover();
	}

	private void RefreshCurrentQuestionWithHover()
	{
		if (queue.Count > 0)
		{
			var currentQueueItem = queue.Last();
			string eventName = (string)currentQueueItem[0];
			var currentQuestion = (Godot.Collections.Dictionary)currentQueueItem[1];
			
			Text = terminalHistory + PrepareQuestionForTerminal(eventName, currentQuestion, true);
			ScrollToLine(GetLineCount() - 1);
		}
	}

	private Godot.Collections.Dictionary RetrieveQuestion(Godot.Collections.Array eventQuestions)
	{
		if (eventQuestions.Count > 0)
		{
			int randomQuestionIndex = rng.RandiRange(0, eventQuestions.Count - 1);
			return eventQuestions[randomQuestionIndex].AsGodotDictionary();
		}
		return null;
	}

	// Start here
	public void handle_event_from_action_event(string eventName, Godot.Collections.Array eventQuestions)
	{
		var currentQuestion = RetrieveQuestion(eventQuestions);
		if (currentQuestion == null)
		{
			return;
		}

		var answers = currentQuestion["answers"].AsGodotArray();
		currentQuestion["answers"] = RandomizeAnswers(answers);
		
		// Push to queue both current event_name and question content
		queue.Add(new object[] { eventName, currentQuestion });
		Text = terminalHistory + PrepareQuestionForTerminal(eventName, currentQuestion, true);
		ScrollActive = false;
		ScrollToLine(GetLineCount() - 1);
	}

	private void UpdateTerminalContent(string eventName, Godot.Collections.Dictionary currentQuestion, int answerIdx)
	{
		terminalHistory += PrepareQuestionForTerminal(eventName, currentQuestion, false, answerIdx);
		
		// Remove the answered question from queue
		if (queue.Count > 0)
		{
			queue.RemoveAt(queue.Count - 1);
		}
		
		// Display next question if available
		if (queue.Count > 0)
		{
			var nextQueueItem = queue.Last();
			string nextEventName = (string)nextQueueItem[0];
			var nextQuestion = (Godot.Collections.Dictionary)nextQueueItem[1];
			Text = terminalHistory + PrepareQuestionForTerminal(nextEventName, nextQuestion, true);
		}
		else
		{
			Text = terminalHistory;
		}
	}

	private void HandleMetaClicked(Variant meta)
	{
		StartSoundSelectedAnswer();
		ScrollActive = true;
		
		string[] questionChoosedInfo = meta.AsString().Split('_'); // questionId_answerIdx
		GD.Print(questionChoosedInfo);
		
		var questionFromQueue = PopSelectedQuestion(questionChoosedInfo[0]);
		GD.Print(questionFromQueue);
		
		if (questionFromQueue.Length == 0) return;
		
		string eventName = (string)questionFromQueue[0];
		var currentQuestion = (Godot.Collections.Dictionary)questionFromQueue[1];
		
		UpdateTerminalContent(eventName, currentQuestion, questionChoosedInfo[1].ToInt());
		
		var node = GetNode("../../../MainControl/" + eventName);
		if (node != null)
		{
			node.Call("RemoveActionEvent");
		}
		
		var answers = currentQuestion["answers"].AsGodotArray();
		var selectedAnswer = answers[questionChoosedInfo[1].ToInt()].AsGodotDictionary();
		
		if (selectedAnswer != null)
		{
			// Add node_name to selected answer for correct behaviour in pause/resume hexagon timer
			selectedAnswer["node_name"] = eventName;
			globals.Set("currentAnswer", selectedAnswer);
			EmitSignal(SignalName.AnswerSignal, selectedAnswer);
		}
	}

	private string PrepareQuestionForTerminal(string eventName, Godot.Collections.Dictionary question, bool withUrl = false, int answeredIdx = -1)
	{
		string questionTitle = question["title"].AsString();
		string contentToAppend = $"[color=red]{eventName}[/color]\n{CheckForCharacters(questionTitle)}\n\n";
		string aftermathColor = "#4A90E2";//"#FFB6C1"; // Light Pink
		string hoverTextColor = "#FFD700"; // Gold - you can change this to any color you prefer
		
		var answers = question["answers"].AsGodotArray();
		
		for (int i = 0; i < answers.Count; i++)
		{
			var answer = answers[i].AsGodotDictionary();
			string answerText = answer["text"].AsString();
			
			if (withUrl)
			{
				string questionId = question["id"].AsString();
				string metaId = $"{questionId}_{i}";
				
				// Check if this answer is currently being hovered
				bool isHovered = currentHoveredMeta == metaId;
				
				if (isHovered)
				{
					// Apply hover text color styling
					contentToAppend += $"{i + 1}. [color={hoverTextColor}][url={metaId}]{answerText}[/url][/color]";
				}
				else
				{
					// Normal styling
					contentToAppend += $"{i + 1}. [url={metaId}]{answerText}[/url]";
				}
			}
			else
			{
				if (i == answeredIdx)
				{
					contentToAppend += $"[color=4A90E2]{i + 1}. {answerText}[/color]";
				}
				else
				{
					contentToAppend += $"{i + 1}. {answerText}";
				}
			}
			contentToAppend += "\n";
		}
		
		if (answeredIdx != -1)
		{
			var selectedAnswer = answers[answeredIdx].AsGodotDictionary();
			if (selectedAnswer.ContainsKey("aftermath"))
			{
				string aftermathText = selectedAnswer["aftermath"].AsString();
				if (!string.IsNullOrEmpty(aftermathText))
				{
					contentToAppend += $"\n[color={aftermathColor}]{aftermathText}[/color]\n";
				}
			}
		}
		
		return contentToAppend + "\n";
	}

	private Godot.Collections.Array RandomizeAnswers(Godot.Collections.Array answers, int amount = 3)
	{
		if (answers.Count <= amount) return answers;
		
		var randomizedAnswers = new Godot.Collections.Array();
		var answersCopy = new Godot.Collections.Array(answers);
		
		for (int i = 0; i < amount; i++)
		{
			int randomIndex = rng.RandiRange(0, answersCopy.Count - 1);
			randomizedAnswers.Add(answersCopy[randomIndex]);
			answersCopy.RemoveAt(randomIndex);
		}
		
		return randomizedAnswers;
	}

	private object[] PopSelectedQuestion(string questionId)
	{
		for (int i = 0; i < queue.Count; i++)
		{
			var questionDict = (Godot.Collections.Dictionary)queue[i][1];
			if (questionId == questionDict["id"].AsString())
			{
				var result = queue[i];
				queue.RemoveAt(i);
				return result;
			}
		}
		return new object[0];
	}

	private string CheckForCharacters(string questionTitle, string personaColor = "#05C9C9")
	{
		string regexPattern = "%(.*?)%";
		regexEngine.Compile(regexPattern);
		var results = regexEngine.SearchAll(questionTitle);
		
		foreach (RegExMatch result in results)
		{
			string matchedString = result.GetString(0);
			GD.Print($"matched_string {matchedString}");
			string matchedStringReplaced = result.GetString(1);
			questionTitle = questionTitle.Replace(matchedString, 
				$"[wave amp=50.0 freq=5.0 connected=1][color={personaColor}]{matchedStringReplaced}[/color][/wave]");
		}
		
		return questionTitle;
	}

	private void StartSoundSelectedAnswer()
	{
		GetNode("../TerminalSelectedAnswerSfx").Call("play");
	}
}
