using Global;
using Godot;
using System;

namespace Dungeon
{
    public partial class Player : CharacterBody2D
    {
        private class Animations
        {
            private Animations() {} // prevent instantiation

            public const string IdleRight = "idle_right";
            public const string RunRight = "run_right";
        }

        [Export]
        public byte TileSize = 20;

        [Export]
        private AnimatedSprite2D sprite; // set in Godot

        [Export]
        private Timer moveTimer; // set in Godot

        [Export]
        private Vector2 facingDir = Controls.MoveDir[Controls.MoveRight];

        private double moveSpeed;

        public override void _Ready()
        {
            moveSpeed = 1 / moveTimer.WaitTime;
            sprite.Play();
        }

        public override void _PhysicsProcess(double delta)
        {
            bool isStopped = moveTimer.IsStopped();
            if (isStopped)
            {
                foreach (var (moveKey, moveDir) in Controls.MoveDir)
                {
                    if (Input.IsActionPressed(moveKey))
                    {
                        if (facingDir == moveDir)
                        {
                            if (TryMove(moveDir))
                            {
                                isStopped = false;
                                break;
                            }
                        }
                        else
                        {
                            FaceDir(moveDir);
                        }
                    }
                }
                if (isStopped)
                {
                    sprite.Play(Animations.IdleRight);
                }
            }
            if (!isStopped)
            {
                MoveAndSlide();
            }
        }

        private void FaceDir(Vector2 direction)
        {
            facingDir = direction;
            // TODO: Change sprite direction
        }

        private bool TryMove(Vector2 direction)
        {
            if (!TestMove(GlobalTransform, direction * TileSize))
            {
                moveTimer.Start();
                double scalar = TileSize * moveSpeed;
                Velocity = new Vector2((float)(facingDir.X * scalar), (float)(facingDir.Y * scalar));

                if (direction == Vector2.Right)
                {
                    sprite.FlipH = false;
                }
                else if (direction == Vector2.Left)
                {
                    sprite.FlipH = true;
                }
                sprite.Play(Animations.RunRight);

                return true;
            }
            return false;
        }
    }
}
