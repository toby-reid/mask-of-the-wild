using Godot;
using System;

namespace Dungeon
{
    public partial class DyingRabbit : Node2D
    {
        private const string DeathAnimation = "death";

        [Export]
        private AnimatedSprite2D sprite;

        [Export]
        private Area2D cutsceneTrigger;

        [Export]
        private Timer breatheAndDie;

        private bool isDying = false;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            if (Global.PersistentData.AvailableMasks.Contains(Global.Masks.RABBIT))
            {
                sprite.Animation = DeathAnimation;
                sprite.Frame = sprite.SpriteFrames.GetFrameCount(DeathAnimation) - 1;
            }
            else
            {
                cutsceneTrigger.BodyEntered += OnBodyEntered;
                breatheAndDie.Timeout += OnBreathEnd;
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            if (isDying)
            {
                if (breatheAndDie.IsStopped())
                {
                    if (!sprite.IsPlaying())
                    {
                        if (!Global.PersistentData.AvailableMasks.Contains(Global.Masks.RABBIT))
                        {
                            CreateDialogue();
                            Global.PersistentData.AvailableMasks.Add(Global.Masks.RABBIT);
                        }
                    }
                }
                else
                {
                    sprite.SpeedScale = (float)((sprite.SpeedScale - (sprite.SpeedScale * 0.9)) * delta);
                }
            }
        }

        private void OnBodyEntered(Node2D body)
        {
            if (body is Player player)
            {
                player.LockMovement();
                breatheAndDie.Start();
                isDying = true;
            }
        }

        private void OnBreathEnd()
        {
            sprite.SpeedScale = 1;
            sprite.Play(DeathAnimation);
        }

        private void CreateDialogue()
        {

        }
    }
}
