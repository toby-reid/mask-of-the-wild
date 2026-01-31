using Godot;
using System;

public partial class PlaceHolderCursorEventListener : Node2D
{
    [Export] public CursorListener[] cursorListeners;
    [Export] public string[] cursorStrings; // Make sure this array length matches cursorListeners
    [Export] public DialoguePlaceholder dialoguePlaceholder;

    [Export] public string ScenePath;
    [Export] public CursorListener SceneListener;
    public override void _Ready()
    {
        // Subscribe to OnClicked for all listeners
        for (int i = 0; i < cursorListeners.Length; i++)
        {
            if (cursorListeners[i] != null)
                cursorListeners[i].OnClicked += HandleCursorClicked;
        }
        if(SceneListener != null)
        {
            SceneListener.OnClicked += HandleSceneClicked;
        }

        // Optional sanity check
        if (cursorListeners.Length != cursorStrings.Length)
            GD.PrintErr("cursorListeners and cursorStrings arrays are not the same length!");
    }

    private void HandleCursorClicked(object? sender, EventArgs e)
    {
        if (sender is not CursorListener clickedListener)
            return;

        // Find which listener was clicked
        for (int i = 0; i < cursorListeners.Length; i++)
        {
            if (cursorListeners[i] == clickedListener)
            {
                // Set the corresponding dialogue
                dialoguePlaceholder.SetDialogue(cursorStrings[i]);
                return;
            }
        }
    }
    private void HandleSceneClicked(object? sender, EventArgs e)
    {
        if (sender is not CursorListener clickedListener)
            return;
        if (clickedListener == SceneListener)
        {
            // Change scene
            GetTree().ChangeSceneToFile(ScenePath);
        }
    }
}
