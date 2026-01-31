using Godot;
using System;

namespace Dungeon
{
    public partial class TimerButton : Timer, IButtonActions
    {
        private Action _closeDoor;

        public void OpenDoor(Node2D body, Action openDoor)
        {
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
