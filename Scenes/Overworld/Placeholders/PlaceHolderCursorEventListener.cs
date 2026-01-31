using Godot;
using System;

public partial class PlaceHolderCursorEventListener : Node2D
{
    [Export] public CursorListener cursorListener;

    public override void _Ready()
    {
        cursorListener.OnClicked += HandleCursorClicked;
    }
    private void HandleCursorClicked(object? sender, EventArgs e)
    {
        GD.Print("CursorListener reported a click event!");
    }
}
