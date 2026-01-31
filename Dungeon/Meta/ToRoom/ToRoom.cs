using Godot;
using System;

namespace Dungeon
{
    public partial class ToRoom : Area2D
    {
        [Export]
        private Vector2 direction = Vector2.Right;

        [Export]
        private string pathToPlayer = "../Player";

        [Export]
        private string pathToRoom;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            if (Global.PersistentData.RoomTransitionDirection == -direction)
            {
                Player player = GetNode<Player>(pathToPlayer);
                player.GlobalPosition = GlobalPosition + (Global.Constants.TileSize * Global.PersistentData.RoomTransitionDirection);
                player.FaceDir(Global.PersistentData.RoomTransitionDirection);
            }
            BodyEntered += OnBodyEntered;

        }

        public void OnBodyEntered(Node2D body)
        {
            if (body is Player)
            {
                Global.PersistentData.RoomTransitionDirection = direction;
                Callable.From(() => GetTree().ChangeSceneToPacked(GD.Load<PackedScene>(pathToRoom))).CallDeferred();
            }
        }
    }
}
