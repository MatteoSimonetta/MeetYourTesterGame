using Godot;
using System;
using Godot.Collections;

public partial class HandleTerminal : RichTextLabel
{
	[Signal]
	public delegate void AnswerSignalEventHandler(Variant answerTarget);

	private Tween tween;
	private RandomNumberGenerator rng = new RandomNumberGenerator();
	private Godot.Collections.Array queue = new Godot.Collections.Array();
	private string terminalHistory = "";
	private RegEx regexEngine = new RegEx();

	public override void _Ready()
	{
		Clear();
		MetaUnderlined = false;
		ScrollFollowing = true;

		tween = GetTree().CreateTween();
	}

	public Variant RetrieveQuestion(Godot.Collections.Array eventQuestions)
	{
		if (eventQuestions.Count > 0)
		{
			int index = rng.RandiRange(0, eventQuestions.Count - 1);
			return eventQuestions[index];
		}
		return default;
	}

	public void HandleEventFromActionEvent(string eventName, Godot.Collections.Array eventQuestions)
	{
		var question = RetrieveQuestion(eventQuestions);
		if (question.VariantType == Variant.Type.Nil)
			return;

		Dictionary questionDict = (Dictionary)question;
		questionDict["answers"] = RandomizeAnswers((Godot.Collections.Array)questionDict["answers"]);

		queue.Add(new Godot.Collections.Array { eventName, questionDict });

		Text = PrepareQuestionForTerminal(eventName, questionDict, true);
		ScrollActive = false;
		ScrollToLine(GetLineCount() - 1);
	}

	public void UpdateTerminalContent(string eventName, Dictionary currentQuestion, int answerIdx)
	{
		terminalHistory += PrepareQuestionForTerminal(eventName, currentQuestion, false, answerIdx);
		Text = terminalHistory;

		if (queue.Count != 0)
			Text = PrepareQuestionForTerminal(eventName, currentQuestion, true);
	}

	public void HandleMetaClicked(Variant meta)
	{
		StartSoundSelectedAnswer();
		ScrollActive = true;

		string[] parts = meta.AsString().Split("_");
		string questionId = parts[0];
		int answerIdx = int.Parse(parts[1]);

		Godot.Collections.Array entry = PopSelectedQuestion(questionId);
		string eventName = entry[0].AsString();
		Dictionary question = (Dictionary)entry[1];

		UpdateTerminalContent(eventName, question, answerIdx);

		var node = GetNodeOrNull($"../../../MainControl/{eventName}");
		if (node != null)
		{
			node.Call("remove_action_event");
		}

		Dictionary selectedAnswer = (Dictionary)((Godot.Collections.Array)question["answers"])[answerIdx];
		if (selectedAnswer != null)
		{
			selectedAnswer["node_name"] = eventName;
			Globals.Instance.CurrentAnswer = selectedAnswer;
			EmitSignal(SignalName.AnswerSignal, selectedAnswer);
		}
	}

	public string PrepareQuestionForTerminal(string eventName, Dictionary question, bool withUrl = false, int answeredIdx = -1)
	{
		string content = $"[color=red]{eventName}[/color]\n{CheckForCharacters(question["title"].AsString())}\n\n";
		Godot.Collections.Array answers = (Godot.Collections.Array)question["answers"];

		for (int i = 0; i < answers.Count; i++)
		{
			Dictionary answer = (Dictionary)answers[i];
			string text = answer["text"].AsString();

			if (withUrl)
			{
				content += $"{i + 1}. [url={question["id"]}_{i}]{text}[/url]";
			}
			else if (i == answeredIdx)
			{
				content += $"[color=green]{i + 1}. {text}[/color]";
			}
			else
			{
				content += $"{i + 1}. {text}";
			}

			content += "\n";
		}

		if (answeredIdx != -1)
		{
			var answeredAnswer = answers[answeredIdx].As<Dictionary>();
			if (answeredAnswer.ContainsKey("aftermath"))
			{
				content += $"[color=#FFB6C1]{answeredAnswer["aftermath"]}[/color]\n";
			}
		}
		
		return content + "\n";
	}

	public Godot.Collections.Array RandomizeAnswers(Godot.Collections.Array answers, int amount = 3)
	{
		if (answers.Count <= amount)
			return answers;

		var randomized = new Godot.Collections.Array();
		for (int i = 0; i < amount; i++)
		{
			int index = rng.RandiRange(0, answers.Count - 1);
			randomized.Add(answers[index]);
			answers.RemoveAt(index);
		}
		return randomized;
	}

	public Godot.Collections.Array PopSelectedQuestion(string questionId)
	{
		for (int i = 0; i < queue.Count; i++)
		{
			var entry = (Godot.Collections.Array)queue[i];
			Dictionary question = entry[1].As<Dictionary>();

			if (question["id"].AsString() == questionId)
			{
				queue.RemoveAt(i);
				return entry;
			}
		}
		return new Godot.Collections.Array();
	}

	public string CheckForCharacters(string title, string personaColor = "#05C9C9")
	{
		regexEngine.Compile("%(.*?)%");
		var results = regexEngine.SearchAll(title);

		foreach (var match in results)
		{
			string fullMatch = match.GetString(0);
			string innerText = match.GetString(1);
			string replacement = $"[wave amp=50.0 freq=5.0 connected=1][color={personaColor}]{innerText}[/color][/wave]";
			title = title.Replace(fullMatch, replacement);
		}

		return title;
	}

	private void StartSoundSelectedAnswer()
	{
		GetNode<AudioStreamPlayer>("../TerminalSelectedAnswerSfx").Play();
	}
}
