using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Global
{
    public class Controls
    {
        // Disallow instantiation
        private Controls() {}

        public const string MoveUp = "move_up";
        public const string MoveDown = "move_down";
        public const string MoveLeft = "move_left";
        public const string MoveRight = "move_right";
        public static readonly ImmutableDictionary<string, Vector2> MoveDir = new Dictionary<string, Vector2>{
            {MoveRight, Vector2.Right},
            {MoveUp, Vector2.Up},
            {MoveLeft, Vector2.Left},
            {MoveDown, Vector2.Down},
        }.ToImmutableDictionary();

        public const string ButtonA = "action_a";
        public const string ButtonB = "action_b";

        public const string Start = "start";
        public const string Select = "select";
    }
}
