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
        private AnimatedSprite2D sprite; // set in Godot

        [Export]
        private Timer changeDirectionTimer; // set in Godot

        [Export]
        public Timer MoveTimer; // set in Godot

        [Export]
        public Vector2 FacingDir { get; private set; } = Controls.MoveDir[Controls.MoveRight];

        [Export]
        private PackedScene cameraShaker; // set in Godot

        [Export]
        private CollisionShape2D collisionShape; // set in Godot

        public double MoveSpeed { get; private set; }

        private Vector2 targetPos;

        private Timer rabbitTimer;

        private Timer deerTimer;
        private double deerDashSpeed;

        public override void _Ready()
        {
            MoveTimer.Timeout += FixPosition;
            MoveSpeed = 1 / MoveTimer.WaitTime;

            rabbitTimer = new();
            rabbitTimer.WaitTime = MoveTimer.WaitTime * 2;
            rabbitTimer.OneShot = true;
            rabbitTimer.Timeout += FixPosition;
            rabbitTimer.Timeout += () => collisionShape.Disabled = false;
            AddChild(rabbitTimer);

            int deerScalar = 5;
            deerDashSpeed = deerScalar * MoveSpeed;
            deerTimer = new();
            deerTimer.WaitTime = MoveTimer.WaitTime / deerScalar;
            deerTimer.OneShot = true;
            deerTimer.Timeout += FixPosition;
            deerTimer.Timeout += () => TryDeerAction(true);
            AddChild(deerTimer);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (CanMove())
            {
                bool canMove = true;
                foreach (var (moveKey, moveDir) in Controls.MoveDir)
                {
                    if (Input.IsActionPressed(moveKey))
                    {
                        if (FacingDir == moveDir)
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
                            break;
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
                Velocity += GetGravity() * (float)delta;
            }
            MoveAndSlide();
        }

        public bool CanMove()
        {
            return changeDirectionTimer.IsStopped()
                && MoveTimer.IsStopped()
                && rabbitTimer.IsStopped()
                && deerTimer.IsStopped();
        }

        public void FaceDir(Vector2 direction)
        {
            FacingDir = direction;
            if (direction == Vector2.Right)
            {
                sprite.FlipH = false;
            }
            else if (direction == Vector2.Left)
            {
                sprite.FlipH = true;
            }
            changeDirectionTimer.Start();
        }

        private bool TryMove()
        {
            if (!TestMove(GlobalTransform, FacingDir * Constants.TileSize))
            {
                MoveTimer.Start();
                targetPos = Position + (Constants.TileSize * FacingDir);
                double scalar = Constants.TileSize * MoveSpeed;
                Velocity = new Vector2((float)(FacingDir.X * scalar), (float)(FacingDir.Y * scalar));

                sprite.Play(Animations.RunActions[PersistentData.CurrentMask]);

                return true;
            }
            return false;
        }

        private bool TryRabbitAction()
        {
            Vector2 newPosition = GlobalPosition + (FacingDir * Constants.TileSize * 2);
            if (IsPositionOnScreen(newPosition) && !TestMove(new Transform2D(GlobalRotation, newPosition), Vector2.Zero))
            {
                rabbitTimer.Start();
                targetPos = Position + (Constants.TileSize * FacingDir * 2);
                double xScalar = Constants.TileSize * MoveSpeed;
                float ySpeed =
                    (FacingDir == Vector2.Up) ? -300
                    : (FacingDir == Vector2.Down) ? -100
                    : -200;
                Velocity = new Vector2((float)(FacingDir.X * xScalar), ySpeed);
                collisionShape.Disabled = true;

                sprite.Play(Animations.RabbitJumpRight);

                return true;
            }
            return false;
        }

        private bool TryDeerAction(bool alreadyDashing = false)
        {
            if (!TestMove(GlobalTransform, FacingDir * Constants.TileSize))
            {
                deerTimer.Start();
                targetPos = Position + (Constants.TileSize * FacingDir);
                double scalar = Constants.TileSize * deerDashSpeed;
                Velocity = new Vector2((float)(FacingDir.X * scalar), (float)(FacingDir.Y * scalar));

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
        }

        private bool IsPositionOnScreen(Vector2 position)
        {
            Camera2D camera = GetViewport().GetCamera2D();
            Vector2 screenPos = camera.GetCanvasTransform() * (position + camera.GlobalPosition);
            Rect2 viewportRect = GetViewport().GetVisibleRect();
            return viewportRect.HasPoint(screenPos);
        }
    }
}
