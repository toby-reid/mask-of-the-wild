using Godot;
using System;

namespace Dungeon
{
    public partial class TimerButton : Timer, IButtonActions
    {
        [Export]
        private Sprite2D sprite;

        private Action _closeDoor;

        public override void _Ready()
        {
            Timeout += () => --sprite.Frame;
        }

        public void OpenDoor(Node2D body, Action openDoor)
        {
            ++sprite.Frame;
            Stop();
            openDoor();
        }

        public void CloseDoor(Node2D body, Action closeDoor)
        {
            Timeout -= _closeDoor;
            Timeout += closeDoor;
            _closeDoor = closeDoor;
            Start();
        }
    }
}
