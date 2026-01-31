using Godot;
using System;

public partial class DialoguePlaceholder : Control
{
    [Export] public RichTextLabel dialogueLabel;
    [Export] public float CharactersPerSecond = 10f;

    private float _charTimer = 0f;
    private bool _isTyping = false;
    private bool _waitingForClick = false;
    private bool _ignoreClickThisFrame = false;

    public event Action DialogueFinished;

    public override void _Process(double delta)
    {
        if (_ignoreClickThisFrame)
        {
            _ignoreClickThisFrame = false; // reset immediately next frame
            return; // skip processing input this frame
        }
        if (_isTyping)
        {
            _charTimer += (float)delta * CharactersPerSecond;

            int charsToShow = (int)_charTimer;
            if (charsToShow > 0)
            {
                dialogueLabel.VisibleCharacters += charsToShow;
                _charTimer -= charsToShow;
            }

            // Finished typing all characters
            if (dialogueLabel.VisibleCharacters >= dialogueLabel.GetTotalCharacterCount())
            {
                _isTyping = false;
                _waitingForClick = true;
                DialogueFinished?.Invoke();
            }

        }
        // Wait for player confirmation
        if (Input.IsActionJustPressed(Global.Controls.AcceptButton) && GameState.IsDialogueActive)
        {
            if (_waitingForClick)
            {
                _waitingForClick = false;
                ResumeGame();
                Hide(); // hide the dialogue box after clicking
            }
            else
            {
                // If not waiting for click, finish typing immediately
                dialogueLabel.VisibleCharacters = dialogueLabel.GetTotalCharacterCount();
                _isTyping = false;
                _waitingForClick = true;
                GD.Print("Dialogue finished by skipping.");
            }
        }

    }

    public void SetDialogue(string text)
    {
        Show(); // show the dialogue box when dialogue starts
        dialogueLabel.ParseBbcode(text);
        dialogueLabel.VisibleCharacters = 0;

        _charTimer = 0f;
        _isTyping = true;
        _waitingForClick = false;
        _ignoreClickThisFrame = true;

        FreezeGame();
    }

    private void FreezeGame()
    {
        GameState.IsDialogueActive = true;
    }

    private void ResumeGame()
    {
        GameState.IsDialogueActive = false;
    }
}
