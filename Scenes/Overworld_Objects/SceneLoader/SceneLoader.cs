using Global;
using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class SceneLoader : Node2D
{
    [Export] public string ScenePath;
    [Export] public CursorListener SceneListener;

    [Export] public bool isRiverDungeon = false;
    [Export] public bool isEndScreen;

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
            if (isRiverDungeon)
                GameState.isRiverVisited = true;
            if (isEndScreen)
            {
                GameState.isRiverVisited = false;
                Global.PersistentData.CurrentMask = Masks.NONE;
                Global.PersistentData.AvailableMasks = [Masks.NONE];
            }
        }
    }

}
