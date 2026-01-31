using Godot;
using System;

public partial class BushListenerEvent : Node2D
{
    [Export] public CursorListener bushListener;
    [Export] public string bushText; // Make sure this array length matches cursorListeners
    [Export] public DialoguePlaceholder dialoguePlaceholder;

    [Export] public string bushDungeonScenePath;

    public override void _Ready()
    {
        // Subscribe to OnClicked for all listeners
        
        if (bushListener != null)
            bushListener.OnClicked += HandleCursorClicked;
        
    }

    private void HandleCursorClicked(object? sender, EventArgs e)
    {
        if (sender is not CursorListener clickedListener)
            return;

        // Find which listener was clicked
        
        if (bushListener == clickedListener)
        {
            if (GameState.isRiverVisited == false)
            {
                // Set the corresponding dialogue
                dialoguePlaceholder.SetDialogue(bushText);
                return;
            }
            else
            {
                //Load the bush dungeon scene
                GetTree().ChangeSceneToFile(bushDungeonScenePath);
            }
        }
        
    }
    
}
