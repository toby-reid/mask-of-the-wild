using Godot;
using System;

public partial class FreeWithoutRabbit : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (!Global.PersistentData.AvailableMasks.Contains(Global.Masks.RABBIT))
        {
            QueueFree();
        }
    }
}
