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

            public const string DeerIdleRight = "deer_idle_right";
            public const string DeerRunRight = "deer_run_right";
            public const string DeerDashRight = "deer_dash_right";

            public static readonly ImmutableDictionary<Masks, string> IdleActions = new Dictionary<Masks, string>{
                {Masks.NONE, IdleRight},
                {Masks.RABBIT, RabbitIdleRight},
                {Masks.DEER, DeerIdleRight},
            }.ToImmutableDictionary();
            public static readonly ImmutableDictionary<Masks, string> RunActions = new Dictionary<Masks, string>{
                {Masks.NONE, RunRight},
                {Masks.RABBIT, RabbitRunRight},
                {Masks.DEER, DeerRunRight},
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

        [Export]
        private PackedScene cameraShaker; // set in Godot

        private Vector2 targetPos;

        private Timer rabbitTimer;

        private Timer deerTimer;
        private double deerDashSpeed;

        private double moveSpeed;

        public override void _Ready()
        {
            moveTimer.Timeout += FixPosition;
            moveSpeed = 1 / moveTimer.WaitTime;

            rabbitTimer = new();
            rabbitTimer.WaitTime = moveTimer.WaitTime * 2;
            rabbitTimer.OneShot = true;
            rabbitTimer.Timeout += FixPosition;
            AddChild(rabbitTimer);

            int deerScalar = 4;
            deerDashSpeed = deerScalar * moveSpeed;
            deerTimer = new();
            deerTimer.WaitTime = moveTimer.WaitTime / deerScalar;
            deerTimer.OneShot = true;
            deerTimer.Timeout += FixPosition;
            deerTimer.Timeout += () => TryDeerAction(true);
            AddChild(deerTimer);
        }

        public override void _PhysicsProcess(double delta)
        {
            bool canMove = moveTimer.IsStopped() && rabbitTimer.IsStopped() && deerTimer.IsStopped();
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
                        case Masks.DEER:
                            if (TryDeerAction())
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

        private bool TryDeerAction(bool alreadyDashing = false)
        {
            if (!TestMove(GlobalTransform, facingDir * TileSize))
            {
                deerTimer.Start();
                targetPos = Position + (TileSize * facingDir);
                double scalar = TileSize * deerDashSpeed;
                Velocity = new Vector2((float)(facingDir.X * scalar), (float)(facingDir.Y * scalar));

                if (sprite.Animation != Animations.DeerDashRight)
                {
                    sprite.Play(Animations.DeerDashRight);
                }

                return true;
            }
            if (alreadyDashing)
            {
                AddChild(cameraShaker.Instantiate());
            }
            return false;
        }

        private bool SelectNextMask()
        {
            Masks currentMask = PersistentData.CurrentMask;
            Masks[] masks = Enum.GetValues<Masks>();
            foreach (Masks mask in masks[((int)currentMask + 1)..])
            {
                if (SetMask(mask))
                {
                    return true;
                }
            }
            foreach (Masks mask in masks[..(int)currentMask])
            {
                if (SetMask(mask))
                {
                    return true;
                }
            }
            return false;
        }

        private bool SetMask(Masks mask)
        {
            if (PersistentData.AvailableMasks.Contains(mask))
            {
                PersistentData.CurrentMask = mask;
                if (mask == Masks.DEER)
                {
                    sprite.Offset = new(0, -10);
                }
                else
                {
                    sprite.Offset = Vector2.Zero;
                }
                return true;
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
