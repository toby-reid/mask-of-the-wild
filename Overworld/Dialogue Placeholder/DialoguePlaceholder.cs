using Godot;
using System;

public partial class DialoguePlaceholder : Control
{
    [Export] public RichTextLabel dialogueLabel;
    [Export] public float CharactersPerSecond = 10f;

    private float _charTimer = 0f;

    public override void _Process(double delta)
    {
        if (dialogueLabel.VisibleCharacters < dialogueLabel.GetTotalCharacterCount())
        {
            _charTimer += (float)delta * CharactersPerSecond;

            int charsToShow = (int)_charTimer;
            if (charsToShow > 0)
            {
                dialogueLabel.VisibleCharacters += charsToShow;
                _charTimer -= charsToShow;
            }
        }
    }

    public void SetDialogue(string text)
    {
        dialogueLabel.ParseBbcode(text);
        dialogueLabel.VisibleCharacters = 0;
        _charTimer = 0f;
    }
}
