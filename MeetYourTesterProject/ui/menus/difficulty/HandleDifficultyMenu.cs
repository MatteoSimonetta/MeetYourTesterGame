using Godot;
using System;

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

    public override void _Ready()
    {
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

    public void OnDiffBtnMouseEntered(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 0:
                cancelIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-cancel-select.svg");
                cancelLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-cancel-select.svg");
                break;
            case 1:
                easyIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-easy-select.svg");
                easyLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-easy-select.svg");
                break;
            case 2:
                mediumIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-medium-select.svg");
                mediumLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-medium-select.svg");
                break;
            case 3:
                hardIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-hard-select.svg");
                hardLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-hard-select.svg");
                break;
        }
    }

    public void OnDiffBtnMouseExited(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 0:
                cancelIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-cancel.svg");
                cancelLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-cancel.svg");
                break;
            case 1:
                easyIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-easy.svg");
                easyLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-easy.svg");
                break;
            case 2:
                mediumIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-medium.svg");
                mediumLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-medium.svg");
                break;
            case 3:
                hardIcon.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-icon-hard.svg");
                hardLabel.TextureNormal = (Texture2D)ResourceLoader.Load("res://images/start-scene/btn-label-hard.svg");
                break;
        }
    }

    public void OnCancelBtnPressed()
    {
        GetTree().ChangeSceneToFile("res://ui/menus/main_menu.tscn");
    }
}
