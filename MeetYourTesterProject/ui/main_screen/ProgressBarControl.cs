using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Linq;

public partial class ProgressBarControl : Control
{
    [Signal] public delegate void DeadlineMissedEventHandler();
    [Signal] public delegate void ProgressBarLimitReachedEventHandler();
    [Signal] public delegate void LastDeadlineMissedEventHandler();

    private Node terminal;
    private readonly string PROGRESS_BAR_DICTIONARY_KEY = "progress_bar";
    private readonly string PROGRESS_BAR_VALUE_DICTIONARY_KEY = "value";
    private readonly string PROGRESS_BAR_ZONE_DICTIONARY_KEY = "zone";

    private PackedScene zonesScene = GD.Load<PackedScene>("res://ui/main_screen/progress_bar_zones_scene.tscn");
    private PackedScene deadlineScene = GD.Load<PackedScene>("res://ui/main_screen/progress_bar_deadline_scene.tscn");

    private List<Dictionary> zonesQueue = new List<Dictionary>();
    private Dictionary zoneDictionary = new Dictionary();
    private string[] colors = { "red", "green" };
    private string[] sizes = { "sm", "md", "lg" };

    public override void _Ready()
    {
        terminal = GetNode("../Terminal/_terminal_mock/terminal_content");
        terminal.Connect("answer_signal", new Callable(this, nameof(ApplyProgressBarEffects)));

        foreach (var color in colors)
        {
            var sizeDict = new Dictionary();
            foreach (var size in sizes)
            {
                sizeDict[size] = GD.Load<Texture2D>($"res://images/main-game/progress-bar/{color}-zone-{size}.svg");
            }
            zoneDictionary[color] = sizeDict;
        }

        CreateDeadlines();
        InitLastDeadlineLabel();
    }

    public override void _Process(double delta)
    {
        if (IsZonePresent())
        {
            int positionBar = GetCurrentPosition();
            if (IsInsideZone(positionBar))
            {
                float speed = (float)zonesQueue[0]["speed"];
                var progressBar = GetNode<TextureProgressBar>("GameProgressBar");
                if (speed < 1)
                    progressBar.TextureProgress = GD.Load<Texture2D>("res://images/main-game/progress-bar/yellow-bars-slower.svg");
                else if (speed > 1)
                    progressBar.TextureProgress = GD.Load<Texture2D>("res://images/main-game/progress-bar/yellow-bars-faster.svg");
            }
            if (positionBar > (int)zonesQueue[0]["end_pos"])
            {
                RemoveZone();
            }
        }
    }

    private bool IsZonePresent() => zonesQueue.Count > 0;

    private bool IsInsideZone(int positionBar)
    {
        return IsZonePresent() &&
               positionBar > (int)zonesQueue[0]["start_pos"] &&
               positionBar < (int)zonesQueue[0]["end_pos"];
    }

    private int GetCurrentPosition()
    {
        var bar = GetNode<TextureProgressBar>("GameProgressBar");
        return GetPixelFromPercent((float)bar.Value, (int)bar.Size.X);
    }

    private int GetPixelFromPercent(float percent, int total) => (int)(percent * total / 100f);

    public void AutoIncrement()
    {
        var bar = GetNode<TextureProgressBar>("GameProgressBar");
        GetNode<Label>("ProgressBarSpeedDbg").Text = bar.Value.ToString();

        float speed = Globals.Instance.ProgressBarSpeed;
        if (IsInsideZone(GetCurrentPosition()))
            speed *= (float)zonesQueue[0]["speed"];

        bar.Value += speed;
        CheckProgressBarLimitReached();
        DecreaseDeadlinesTimers();
    }

    private void CheckProgressBarLimitReached()
    {
        var bar = GetNode<TextureProgressBar>("GameProgressBar");
        if (bar.Value >= bar.MaxValue)
        {
            GD.Print("GAME OVER: progress bar has reached 100%");
            EmitSignal(SignalName.ProgressBarLimitReached);
        }
    }

    private void ApplyProgressBarEffects(Dictionary selectedAnswer)
    {
        GD.Print("apply_progress_bar_effects");

        if (selectedAnswer.ContainsKey(PROGRESS_BAR_DICTIONARY_KEY))
        {
            var effect = (Dictionary)selectedAnswer[PROGRESS_BAR_DICTIONARY_KEY];

            if (effect.ContainsKey(PROGRESS_BAR_VALUE_DICTIONARY_KEY))
            {
                GetNode<TextureProgressBar>("GameProgressBar").Value += (float)effect[PROGRESS_BAR_VALUE_DICTIONARY_KEY];
                CheckProgressBarLimitReached();
            }

            if (effect.ContainsKey(PROGRESS_BAR_ZONE_DICTIONARY_KEY))
            {
                CreateZone((Dictionary)effect[PROGRESS_BAR_ZONE_DICTIONARY_KEY]);
            }
        }
    }

    private void InitLastDeadlineLabel()
    {
        var bar = GetNode<TextureProgressBar>("GameProgressBar");
        float lastTime = (float)(bar.MaxValue / Globals.Instance.ProgressBarSpeed);
        GetNode<Label>("FinalDeadlineLabel").Text = Utils.FloatToTime(lastTime);
    }

    private void CreateDeadlines()
    {
        var bar = GetNode<TextureProgressBar>("GameProgressBar");
        for (int i = 0; i < Globals.Instance.Deadlines.Count; i++)
        {
            var key = $"deadline_{i}";
            var deadlineEntry = (Dictionary)Globals.Instance.Deadlines[i];
            GD.Print($"Deadline entry {i}: {deadlineEntry}");

            if (!deadlineEntry.ContainsKey(key))
            {
                GD.PushWarning($"Missing expected key '{key}' in Globals.Instance.Deadlines[{i}]. Skipping entry.");
                continue;
            }

            var deadlineData = deadlineEntry[key];
            var deadlineDict = (Dictionary)deadlineData;

            var newDeadline = (Control)deadlineScene.Instantiate();
            float deadlineSeconds = (float)deadlineDict["deadline_position_in_seconds"];
            newDeadline.GetChild<Label>(0).Text = Utils.FloatToTime(deadlineSeconds);

            float posX = deadlineSeconds * Globals.Instance.ProgressBarSpeed / 100 * bar.Size.X;
            posX -= newDeadline.GetChild<TextureRect>(1).Size.X;
            newDeadline.Position = new Vector2(posX, newDeadline.Position.Y);

            GetNode<Control>("DeadlinesContainer").AddChild(newDeadline);
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

    private bool IsDeadlineReached(int index)
    {
        TextureProgressBar bar = GetNode<TextureProgressBar>("GameProgressBar");
        Control frame = GetNode<Control>("ProgressFrame");
        float progressPixel = ((float)bar.Value / (float)bar.MaxValue) * bar.Size.X;
        float offset = bar.Position.X - frame.Position.X;
        var deadline = GetNode<Control>("DeadlinesContainer").GetChild<Control>(index);

        return progressPixel >= deadline.Position.X + deadline.Size.X +
               GetNode<Control>("DeadlinesContainer").Position.X + frame.Position.X + offset;
    }

    private void DecreaseDeadlinesTimers()
    {
        var finalLabel = GetNode<Label>("FinalDeadlineLabel");
        if (finalLabel.Text == "00:00")
        {
            MissedLastDeadline();
            return;
        }

        float timeLeft = (float)((GetNode<TextureProgressBar>("GameProgressBar").MaxValue / Globals.Instance.ProgressBarSpeed) - Globals.Instance.GameTime);
        GD.Print(Globals.Instance.GameTime);
        GD.Print(timeLeft);

        finalLabel.Text = Utils.FloatToTime(timeLeft);

        var container = GetNode<Control>("DeadlinesContainer");

        for (int i = 0; i < container.GetChildCount(); i++)
        {
            var key = $"deadline_{i}";
            var deadline = container.GetChild<Control>(i);
            var label = deadline.GetChild<Label>(0);
            var texture = deadline.GetChild<TextureRect>(1);

            float newTime = (float)(Utils.TimeToSeconds(label.Text) - 1);

            if (texture.Texture.ResourcePath.Contains("missed") || texture.Texture.ResourcePath.Contains("reached"))
                continue;

            if (newTime <= 0)
            {
                newTime = 0;
                if (!IsDeadlineReached(i))
                {
                    texture.Texture = GD.Load<Texture2D>("res://images/main-game/progress-bar/deadline-missed.svg");
                    AddChargeToSabotage();
                }
                else
                {
                    texture.Texture = GD.Load<Texture2D>("res://images/main-game/progress-bar/deadline-reached.svg");
                }
            }
            else
            {
                if (IsDeadlineReached(i))
                    texture.Texture = GD.Load<Texture2D>("res://images/main-game/progress-bar/deadline-reached.svg");
            }

            label.Text = Utils.FloatToTime(newTime);
        }
    }

    private void CreateZone(Dictionary zoneEffects)
    {
        var newZoneScene = zonesScene.Instantiate();
        var newZoneNode = (TextureRect)newZoneScene.GetChild(0);
        ((Node)newZoneScene).RemoveChild(newZoneNode);

        SetZoneTexture(newZoneNode, zoneEffects);
        var newZone = SetNewZoneProperties(zoneEffects, newZoneNode);
        AddZoneToQueueAndContainer(newZone, newZoneNode);
    }

    private void SetZoneTexture(TextureRect node, Dictionary effects)
    {
        int length = effects.ContainsKey("length")
            ? (int)(float)effects["length"]
            : (int)Globals.ProgressBarZoneLength.SMALL;

        float speed = effects.ContainsKey("speedValue")
            ? (float)effects["speedValue"]
            : 1f;

        string color = speed < 1 ? "red" : "green";

        var textures = (Dictionary)zoneDictionary[color];
        switch ((Globals.ProgressBarZoneLength)length)
        {
            case Globals.ProgressBarZoneLength.SMALL:
                node.Texture = (Texture2D)textures["sm"];
                break;
            case Globals.ProgressBarZoneLength.MEDIUM:
                node.Texture = (Texture2D)textures["md"];
                break;
            case Globals.ProgressBarZoneLength.LARGE:
                node.Texture = (Texture2D)textures["lg"];
                break;
            default:
                GD.Print($"Unrecognized zone length: {length}");
                break;
        }
    }

    private Dictionary SetNewZoneProperties(Dictionary effects, TextureRect node)
    {
        var bar = GetNode<TextureProgressBar>("GameProgressBar");

        float offset = effects.ContainsKey("offset")
            ? (float)effects["offset"]
            : 0f;

        float speed = effects.ContainsKey("speedValue")
            ? (float)effects["speedValue"]
            : 1f;

        float start = offset + (IsZonePresent() ? (float)zonesQueue[^1]["end_pos"] : (float)bar.Value);
        float end = start + node.Texture.GetWidth();

        var newZone = new Dictionary
        {
            ["speed"] = speed,
            ["start_pos"] = start,
            ["end_pos"] = end
        };

        node.OffsetLeft = start;
        return newZone;
    }

    private void AddZoneToQueueAndContainer(Dictionary newZone, TextureRect node)
    {
        var container = GetNode<Control>("ZonesContainer");
        if ((float)newZone["end_pos"] <= container.Size.X)
        {
            container.AddChild(node);
            zonesQueue.Add(newZone);
        }
    }

    private void RemoveZone()
    {
        var container = GetNode<Control>("ZonesContainer");
        container.RemoveChild(container.GetChild(0));
        zonesQueue.RemoveAt(0);

        GetNode<TextureProgressBar>("GameProgressBar").TextureProgress =
            GD.Load<Texture2D>("res://images/main-game/progress-bar/yellow-bars-neutral.svg");
    }
}
