using Godot;
using System.Collections.Generic;

namespace Dungeon
{
    public partial class CopyCat : CharacterBody2D
    {
        private const string IdleLeft = "idle_left";
        private const string WalkLeft = "walk_left";

        private static bool hasBeenDefeated = false;

        [Export]
        private AnimatedSprite2D sprite; // set in Godot

        [Export]
        private string playerPath = "../Player";

        private Player player;

        private Vector2 targetPos;

        public override void _Ready()
        {
            player = GetNode<Player>(playerPath);
            if (hasBeenDefeated || !Global.PersistentData.AvailableMasks.Contains(Global.Masks.RABBIT))
            {
                QueueFree();
            }
            else
            {
                player.MoveTimer.Timeout += FixPosition;
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (player.CanMove())
            {
                bool canMove = true;
                foreach (var (moveKey, moveDir) in Global.Controls.MoveDir)
                {
                    if (Input.IsActionPressed(moveKey) && player.FacingDir == moveDir)
                    {
                        if (TryMove(-moveDir))
                        {
                            canMove = false;
                            break;
                        }
                    }
                }
                if (canMove)
                {
                    sprite.Play(IdleLeft);
                    Velocity = Vector2.Zero;
                }
            }
            MoveAndSlide();
        }

        private bool TryMove(Vector2 direction)
        {
            if (!TestMove(GlobalTransform, direction * Global.Constants.TileSize))
            {
                if (player.MoveTimer.IsStopped())
                {
                    player.MoveTimer.Start();
                }
                targetPos = Position + (Global.Constants.TileSize * direction);
                double scalar = Global.Constants.TileSize * player.MoveSpeed;
                Velocity = new Vector2((float)(direction.X * scalar), (float)(direction.Y * scalar));

                if (direction == Vector2.Right)
                {
                    sprite.FlipH = true;
                }
                else if (direction == Vector2.Left)
                {
                    sprite.FlipH = false;
                }
                sprite.Play(WalkLeft);

                return true;
            }
            return false;
        }

        private void FixPosition()
        {
            Position = targetPos;
        }
    }
}
