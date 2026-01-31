using Godot;
using System;

public partial class PlaceHolderCursorEventListener : Node2D
{
    [Export] public CursorListener[] cursorListeners;
    [Export] public string[] cursorStrings; // Make sure this array length matches cursorListeners
    [Export] public DialoguePlaceholder dialoguePlaceholder;

    
    public override void _Ready()
    {
        // Subscribe to OnClicked for all listeners
        for (int i = 0; i < cursorListeners.Length; i++)
        {
            if (cursorListeners[i] != null)
                cursorListeners[i].OnClicked += HandleCursorClicked;
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
    
}
