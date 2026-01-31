using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class RiverSceneLoader : Node2D
{
    [Export] public string ScenePath;
    [Export] public CursorListener SceneListener;


    public override void _Ready()
    {
        if (SceneListener != null)
            SceneListener.OnClicked += HandleSceneClicked;
    }
    private void HandleSceneClicked(object? sender, EventArgs e)
    {
        if (sender is not CursorListener clickedListener)
            return;
        if (clickedListener == SceneListener)
        {
            // Change scene
            GetTree().ChangeSceneToFile(ScenePath);
            GameState.isRiverVisited = true;
        }
    }

}
