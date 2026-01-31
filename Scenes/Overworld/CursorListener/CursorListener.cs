using Godot;
using System;
using System.Threading.Tasks;

public partial class CursorListener : Area2D
{
    [Export] public float selectDuration = .25f;

    private Cursor currentCursor = null;
    private bool cursorInside = false;

    public event EventHandler OnClicked;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.IsInGroup("cursor"))
        {
            cursorInside = true;
            currentCursor = area.GetParent<Node2D>() as Cursor;

            currentCursor?.ShowSelectable();
        }
    }

    private void OnAreaExited(Area2D area)
    {
        if (area.IsInGroup("cursor"))
        {
            cursorInside = false;

            currentCursor?.ShowNormal();
            currentCursor = null;
        }
    }

    public override void _Process(double delta)
    {
        if (cursorInside && Input.IsActionJustPressed("click"))
        {

            if (currentCursor != null)
            {
                // Call the helper async method
                SelectCursorTemporarily(currentCursor);
            }

            OnClicked?.Invoke(this, EventArgs.Empty);
        }
    }

    // Async helper to handle the delay
    private async void SelectCursorTemporarily(Cursor cursor)
    {
        cursor.ShowSelect();

        // Wait 1 second
        await ToSignal(GetTree().CreateTimer(selectDuration), "timeout");

        cursor.ShowNormal();
    }
}
