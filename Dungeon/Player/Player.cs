using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Global;
using Godot;

namespace Dungeon
{
    public partial class Player : CharacterBody2D
    {
        private class Animations
        {
            private Animations() {} // prevent instantiation

            public const string IdleRight = "idle_right";
            public const string RunRight = "run_right";

            public const string RabbitIdleRight = "rabbit_idle_right";
            public const string RabbitRunRight = "rabbit_run_right";
            public const string RabbitJumpRight = "rabbit_jump_right";

            public static readonly ImmutableDictionary<Masks, string> IdleActions = new Dictionary<Masks, string>{
                {Masks.NONE, IdleRight},
                {Masks.RABBIT, RabbitIdleRight},
            }.ToImmutableDictionary();
            public static readonly ImmutableDictionary<Masks, string> RunActions = new Dictionary<Masks, string>{
                {Masks.NONE, RunRight},
                {Masks.RABBIT, RabbitRunRight},
            }.ToImmutableDictionary();
        }

        [Export]
        public byte TileSize = 20;

        [Export]
        private AnimatedSprite2D sprite; // set in Godot

        [Export]
        private Timer moveTimer; // set in Godot

        [Export]
        private Vector2 facingDir = Controls.MoveDir[Controls.MoveRight];

        private Vector2 targetPos;

        private Timer rabbitTimer;

        private double moveSpeed;

        public override void _Ready()
        {
            moveTimer.Timeout += FixPosition;
            rabbitTimer = new();
            rabbitTimer.WaitTime = moveTimer.WaitTime * 2;
            rabbitTimer.OneShot = true;
            rabbitTimer.Timeout += FixPosition;
            AddChild(rabbitTimer);
            moveSpeed = 1 / moveTimer.WaitTime;
            sprite.Play();
        }

        public override void _PhysicsProcess(double delta)
        {
            bool canMove = moveTimer.IsStopped() && rabbitTimer.IsStopped();
            if (canMove)
            {
                foreach (var (moveKey, moveDir) in Controls.MoveDir)
                {
                    if (Input.IsActionPressed(moveKey))
                    {
                        if (facingDir == moveDir)
                        {
                            if (TryMove())
                            {
                                canMove = false;
                                break;
                            }
                        }
                        else
                        {
                            FaceDir(moveDir);
                        }
                    }
                }
                if (canMove && Input.IsActionJustPressed(Controls.ActionButton))
                {
                    switch (PersistentData.CurrentMask)
                    {
                        case Masks.NONE:
                            break;
                        case Masks.RABBIT:
                            if (TryRabbitAction())
                            {
                                canMove = false;
                            }
                            break;
                        // TODO: Implement other masks
                    }
                }
                if (canMove && Input.IsActionJustPressed(Controls.Select))
                {
                    SelectNextMask();
                }
                if (canMove)
                {
                    sprite.Play(Animations.IdleActions[PersistentData.CurrentMask]);
                    Velocity = Vector2.Zero;
                }
            }
            else if (!rabbitTimer.IsStopped())
            {
                Velocity += GetGravity();
            }
            MoveAndSlide();
        }

        private void FaceDir(Vector2 direction)
        {
            facingDir = direction;
            if (direction == Vector2.Right)
            {
                sprite.FlipH = false;
            }
            else if (direction == Vector2.Left)
            {
                sprite.FlipH = true;
            }
        }

        private bool TryMove()
        {
            if (!TestMove(GlobalTransform, facingDir * TileSize))
            {
                moveTimer.Start();
                targetPos = Position + (TileSize * facingDir);
                double scalar = TileSize * moveSpeed;
                Velocity = new Vector2((float)(facingDir.X * scalar), (float)(facingDir.Y * scalar));

                sprite.Play(Animations.RunActions[PersistentData.CurrentMask]);

                return true;
            }
            return false;
        }

        private bool TryRabbitAction()
        {
            if (!TestMove(GlobalTransform, facingDir * TileSize * 2))
            {
                rabbitTimer.Start();
                targetPos = Position + (TileSize * facingDir * 2);
                double xScalar = TileSize * moveSpeed;
                float ySpeed =
                    (facingDir == Vector2.Up) ? -160
                    : (facingDir == Vector2.Down) ? -80
                    : -120;
                Velocity = new Vector2((float)(facingDir.X * xScalar), ySpeed);

                sprite.Play(Animations.RabbitJumpRight);

                return true;
            }
            return false;
        }

        private static bool SelectNextMask()
        {
            Masks currentMask = PersistentData.CurrentMask;
            Masks[] masks = Enum.GetValues<Masks>();
            foreach (Masks mask in masks[((int)currentMask + 1)..])
            {
                if (PersistentData.AvailableMasks.Contains(mask))
                {
                    PersistentData.CurrentMask = mask;
                    return true;
                }
            }
            foreach (Masks mask in masks[..(int)currentMask])
            {
                if (PersistentData.AvailableMasks.Contains(mask))
                {
                    PersistentData.CurrentMask = mask;
                    return true;
                }
            }
            return false;
        }

        private void FixPosition()
        {
            Position = targetPos;
            GD.Print(Position);
        }
    }
}
