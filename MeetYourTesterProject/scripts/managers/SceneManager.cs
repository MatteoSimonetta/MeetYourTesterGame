using Godot;
using System;

public partial class SceneManager : Node
{
    private string currentScenePath = "";
    private string previousScenePath = "";
    
    public static SceneManager Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }

    public void ChangeScene(string newScenePath)
    {
        previousScenePath = currentScenePath;
        currentScenePath = newScenePath;
        GetTree().ChangeSceneToFile(newScenePath);
    }

    public void GoBack()
    {
        if (!string.IsNullOrEmpty(previousScenePath))
        {
            ChangeScene(previousScenePath);
        }
    }
}
