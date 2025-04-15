using Godot;
using System;

public partial class HandleMainMenu : Node
{
    private TextureButton startIcon;
    private TextureButton startLabel;
    private TextureButton quitIcon;
    private TextureButton quitLabel;
    private Control exitMenu;

    [Signal]
    public delegate void QuitSignalEventHandler();

    public override void _Ready()
    {
        startIcon = GetNode<TextureButton>("GridContainer/StartIcon");
        startLabel = GetNode<TextureButton>("GridContainer/CenterStartLabel/StartLabel");
        quitIcon = GetNode<TextureButton>("GridContainer/QuitIcon");
        quitLabel = GetNode<TextureButton>("GridContainer/CenterQuitLabel/QuitLabel");
        exitMenu = GetNode<Control>("ExitMenuControl");
        exitMenu.Visible = false;

        this.Connect("QuitSignal", new Callable(this, nameof(DisableEverything)));
        GetNode("ExitMenuControl/exit_menu").Connect("ResumeFromQuitPrompt", new Callable(this, nameof(EnableEverything)));

        var config = new ConfigFile();
        var err = config.Load("user://settings.cfg");

        if (err == Error.Ok)
        {
            var showPopup = config.GetValue("FirstStart", "show_popup", true);
            if ((bool)showPopup != false)
            {
                ShowTutorialPopup();
                config.SetValue("FirstStart", "show_popup", false);
                config.Save("user://settings.cfg");
            }
        }
        else
        {
            config.SetValue("FirstStart", "show_popup", false);
            config.Save("user://settings.cfg");
            ShowTutorialPopup();
        }
    }

    public override void _Process(double delta) { }

    private void ShowTutorialPopup()
    {
        GetNode<Control>("TutorialPopup").Visible = true;
    }

    private void DisableEverything()
    {
        Utils.Pause(GetNode("GridContainer"));
        exitMenu.Visible = true;

        Utils.ToggleButtonEffect(startIcon);
        Utils.ToggleButtonEffect(startLabel);
        Utils.ToggleButtonEffect(quitIcon);
        Utils.ToggleButtonEffect(quitLabel);
    }

    private void EnableEverything()
    {
        Utils.Unpause(GetNode("GridContainer"));
        exitMenu.Visible = false;

        Utils.ToggleButtonEffect(startIcon);
        Utils.ToggleButtonEffect(startLabel);
        Utils.ToggleButtonEffect(quitIcon);
        Utils.ToggleButtonEffect(quitLabel);
    }

    private void OnStartGameLabelPressed()
    {
        DebugPrint("Start Game Button (label) pressed");
        GetTree().ChangeSceneToFile("res://ui/menus/difficulty/diff_scene.tscn");
    }

    private void OnStartLabelMouseEntered()
    {
        DebugPrint("Start Game Button (label) on hover entered");
        if (!exitMenu.Visible)
        {
            startIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-start-select.svg");
        }
    }

    private void OnStartLabelMouseExited()
    {
        DebugPrint("Start Game Button (label) on hover exited");
        startIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-start.svg");
    }

    private void OnStartIconMouseEntered()
    {
        DebugPrint("Start Game Button (icon) on hover entered");
        if (!exitMenu.Visible)
        {
            startLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-start-select.svg");
        }
    }

    private void OnStartIconMouseExited()
    {
        DebugPrint("Start Game Button (icon) on hover exited");
        startLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-start.svg");
    }

    private void OnQuitLabelPressed()
    {
        DebugPrint("Quit Game Button (label) pressed");
        EmitSignal(SignalName.QuitSignal);
    }

    private void OnQuitIconMouseEntered()
    {
        DebugPrint("Quit Game Button (icon) on hover entered");
        if (!exitMenu.Visible)
        {
            quitLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-quit-select.svg");
        }
    }

    private void OnQuitIconMouseExited()
    {
        DebugPrint("Quit Game Button (icon) on hover exited");
        quitLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-quit.svg");
    }

    private void OnQuitLabelMouseEntered()
    {
        DebugPrint("Quit Game Button (label) on hover entered");
        if (!exitMenu.Visible)
        {
            quitIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-exit-select.svg");
        }
    }

    private void OnQuitLabelMouseExited()
    {
        DebugPrint("Quit Game Button (label) on hover exited");
        quitIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-exit.svg");
    }

    private void DebugPrint(string msg)
    {
        if (Globals.DEBUG_MODE)
        {
            GD.Print(msg);
        }
    }
}
