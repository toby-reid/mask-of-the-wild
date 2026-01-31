using Godot;
using System;

namespace Dungeon
{
    public partial class DialogueBox : Node2D
    {
        [Export]
        private Label textArea;

        [Export]
        private string[] text;

        private ushort textIndex = 0;

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed(Global.Controls.ButtonA) || @event.IsActionPressed(Global.Controls.ButtonB))
            {
                if (++textIndex == text.Length)
                {
                    QueueFree();
                }
                else
                {
                    textArea.Text = text[textIndex];
                }
            }
        }

        public void SetText(string[] newText)
        {
            text = newText;
            textIndex = 0;
            textArea.Text = text.IsEmpty() ? "" : text[0];
        }
    }
}
