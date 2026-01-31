using Godot;
using System;

namespace Dungeon
{
    public interface IButtonActions
    {
        public void OpenDoor(Node2D body, Action openDoor);

        public void CloseDoor(Node2D body, Action closeDoor);
    }

    public partial class Button : Area2D, IDoorOpener, IDoorCloser
    {
        public event Action OnDoorOpened;

        public event Action OnDoorClosed;

        [Export]
        private string buttonActionsName = "ButtonActions";

        private IButtonActions actions;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            actions = GetNode<IButtonActions>(buttonActionsName);
            BodyEntered += OnBodyEntered;
            BodyExited += OnBodyExited;
        }

        private void OnBodyEntered(Node2D body)
        {
            actions.OpenDoor(body, OnDoorOpened);
        }

        private void OnBodyExited(Node2D body)
        {
            actions.CloseDoor(body, OnDoorClosed);
        }
    }
}
