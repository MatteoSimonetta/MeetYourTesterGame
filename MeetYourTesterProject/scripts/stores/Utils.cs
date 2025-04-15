using Godot;
using System;

public partial class Utils : Node
{
    public static string FloatToTime(float secondsFloat = 0)
    {
        int minutes = (int)(secondsFloat / 60) % 60;
        int seconds = (int)secondsFloat % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public static float TimeToSeconds(string timeString)
    {
        var timeComponents = timeString.Split(":");
        int minutes = int.Parse(timeComponents[0]);
        int seconds = int.Parse(timeComponents[1]);
        return minutes * 60 + seconds;
    }

    public static void Pause(Node node)
    {
        node.ProcessMode = Node.ProcessModeEnum.Disabled;
    }

    public static void Unpause(Node node)
    {
        node.ProcessMode = Node.ProcessModeEnum.Inherit;
    }

    public static void ToggleButtonEffect(TextureButton button)
    {
        string hoverPath = button.TextureHover.ResourcePath;

        if (hoverPath.Contains("-select"))
        {
            var splitTexture = hoverPath.Split("-select");
            var basePath = $"{splitTexture[0]}.svg";
            button.TextureHover = (Texture2D)ResourceLoader.Load(basePath);
            button.TextureNormal = (Texture2D)ResourceLoader.Load(basePath);
        }
        else
        {
            var splitTexture = hoverPath.Split(".");
            var normalPath = button.TextureNormal.ResourcePath;
            var hoverSelectPath = $"{splitTexture[0]}-select.svg";
            button.TextureNormal = (Texture2D)ResourceLoader.Load(normalPath);
            button.TextureHover = (Texture2D)ResourceLoader.Load(hoverSelectPath);
        }
    }
}
