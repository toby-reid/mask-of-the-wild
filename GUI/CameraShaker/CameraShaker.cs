using Godot;
using System;

public partial class CameraShaker : Node2D
{
    [Export]
    private Timer expiration; // set in Godot

    private Camera2D camera;

    private Vector2 baseOffset;

    private readonly Random randomizer = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        expiration.Timeout += OnExpiration;
        expiration.Timeout += QueueFree;
        camera = GetViewport().GetCamera2D();
        if (camera == null)
        {
            QueueFree();
        }
        else
        {
            baseOffset = camera.Offset;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        camera.Offset = baseOffset + new Vector2(GetRandomFloat(), GetRandomFloat());
    }

    private void OnExpiration()
    {
        camera.Offset = baseOffset;
    }

    private float GetRandomFloat()
    {
        return (float)(4 * (randomizer.NextDouble() - 0.5));
    }
}
