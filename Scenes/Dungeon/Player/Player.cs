using Global;
using Godot;
using System;

namespace Dungeon
{
    public partial class Player : CharacterBody2D
    {
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
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {

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
                            GD.Print(Position);
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
            }
            if (!isStopped)
            {
                MoveAndSlide();
                GD.Print(Position);
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
                return true;
            }
            return false;
        }
    }
}
