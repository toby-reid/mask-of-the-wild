using Godot;
using System;

public partial class EndScreenController : Node2D
{
    [Export] public DialoguePlaceholder EndDialogue;
    [Export(PropertyHint.MultilineText)]
    public string endDialogue;
    public override void _Ready()
    {
        if (EndDialogue != null)
        {
            EndDialogue.SetDialogue(endDialogue);
        }
    }

}
