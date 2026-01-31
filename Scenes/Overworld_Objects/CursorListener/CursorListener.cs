using Godot;
using System;
using System.Threading.Tasks;

public partial class CursorListener : Area2D
{
    [Export] public float selectDuration = .25f;

    protected Cursor currentCursor = null;
    protected bool cursorInside = false;

    // Textures that can change if the cursor moves over them
    [Export] public Sprite2D sprite = null;
    [Export] public Texture2D normalTexture = null;
    [Export] public Texture2D selectableTexture = null;

    public event EventHandler OnClicked;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
    }

    protected void OnAreaEntered(Area2D area)
    {
        if (area.IsInGroup("cursor"))
        {
            cursorInside = true;
            currentCursor = area.GetParent<Node2D>() as Cursor;

            currentCursor?.ShowSelectable();
        }
    }

    protected void OnAreaExited(Area2D area)
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
        if (sprite != null && normalTexture != null && selectableTexture != null)
        {
            if (!cursorInside)
            {
                sprite.Texture = normalTexture;
            }
            else
            {
                sprite.Texture = selectableTexture;
            }
        }

        if (cursorInside && !GameState.IsDialogueActive && Input.IsActionJustPressed(Global.Controls.AcceptButton))
        {
            if (currentCursor != null)
                SelectCursorTemporarily(currentCursor);

            RaiseClicked();
        }
    }

    // Async helper to handle the delay
    protected virtual async void SelectCursorTemporarily(Cursor cursor)
    {
        // Immediately show select state
        cursor.ShowSelect();

        // Wait 1 second
        await ToSignal(GetTree().CreateTimer(selectDuration), "timeout");

        // SAFETY CHECKS AFTER AWAIT
        if (!IsInstanceValid(cursor))
            return;

        // If cursor left the area or another cursor replaced it
        if (!cursorInside || currentCursor != cursor)
        {
            cursor.ShowNormal();
            return;
        }

        // Cursor is still inside → selectable
        cursor.ShowSelectable();
    }
    protected void RaiseClicked()
    {
        OnClicked?.Invoke(this, EventArgs.Empty);
    }
}
