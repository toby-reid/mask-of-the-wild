using Godot;
using System;

public partial class DrawTextBox : Node2D
{
    [Export]
    private Label text;

    private Rect2 outlineRect;
    private Rect2 rect;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Rect2 viewportRect = GetViewport().GetVisibleRect();
        double margins = viewportRect.Size.X * 0.05;
        double ySize = viewportRect.Size.Y * 0.3;
        rect = new(
            (float)(viewportRect.Position.X + margins),
            (float)(viewportRect.Position.Y + viewportRect.Size.Y - margins - ySize),
            (float)(viewportRect.Size.X * 0.9),
            (float)ySize
        );
        text.Position = rect.Position;
        text.Size = rect.Size;
        int outlineDiff = 5;
        Vector2 outline = new(outlineDiff, outlineDiff);
        outlineRect = new(rect.Position - outline, rect.Size + (2 * outline));
    }

    public override void _Draw()
    {
        DrawRect(outlineRect, Colors.White);
        DrawRect(rect, Colors.Black);
    }
}
