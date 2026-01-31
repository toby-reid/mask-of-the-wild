using Godot;
using System;

namespace Dungeon
{
    public partial class StayDownButton : Node, IButtonActions
    {
        public void OpenDoor(Node2D body, Action openDoor)
        {
            openDoor();
        }

        public void CloseDoor(Node2D body, Action closeDoor)
        {
            // noop
        }
    }
}
