using Godot;
using System.Collections.Generic;

namespace Global
{
    public class PersistentData
    {
        private PersistentData() {}

        public static Masks CurrentMask = Masks.NONE;
        public static HashSet<Masks> AvailableMasks = [Masks.NONE, Masks.RABBIT, Masks.DEER];

        public static Vector2 RoomTransitionDirection = Vector2.Right;

        public static void ChangeScene(SceneTree tree, PackedScene scene)
        {
            tree.ChangeSceneToPacked(scene);
        }
    }
}
