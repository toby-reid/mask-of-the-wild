using Godot;
using System;

namespace Dungeon
{
    public partial class StayDownButton : Node, IButtonActions
    {
        [Export]
        private Sprite2D sprite;

        public void OpenDoor(Node2D body, Action openDoor)
        {
            ++sprite.Frame;
            openDoor();
        }

        public void CloseDoor(Node2D body, Action closeDoor)
        {
            // noop
        }
    }
}
