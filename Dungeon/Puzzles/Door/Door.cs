using Godot;
using System;

namespace Dungeon
{
    public interface IDoorOpener
    {
        event Action OnDoorOpened;
    }

    public interface IDoorCloser
    {
        event Action OnDoorClosed;
    }

    public partial class Door : StaticBody2D
    {
        [Export]
        private string doorOpenerName = "DoorOpener"; // set when this object is used

        [Export]
        private string doorCloserName = "DoorOpener"; // set when this object is used, if necessary

        [Export]
        private CollisionShape2D collision; // set in Godot

        [Export]
        private Sprite2D sprite;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            if (GetNodeOrNull<IDoorOpener>(doorOpenerName) is IDoorOpener opener)
            {
                opener.OnDoorOpened += OpenDoor;
            }
            if (GetNodeOrNull<IDoorCloser>(doorCloserName) is IDoorCloser closer)
            {
                closer.OnDoorClosed += CloseDoor;
            }
        }

        private void OpenDoor()
        {
            Callable.From(() => collision.Disabled = true).CallDeferred();
            sprite.Visible = false;
        }

        private void CloseDoor()
        {
            Callable.From(() => collision.Disabled = false).CallDeferred();
            sprite.Visible = true;
        }
    }
}
