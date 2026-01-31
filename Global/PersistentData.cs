using Godot;
using System.Collections.Generic;

namespace Global
{
    public class PersistentData
    {
        private PersistentData() {}

        public static Masks CurrentMask = Masks.RABBIT;
        public static HashSet<Masks> AvailableMasks = [Masks.NONE];

        public static Vector2 RoomTransitionDirection = Vector2.Right;
    }
}
