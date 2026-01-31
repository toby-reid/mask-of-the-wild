using Godot;
using System;

public partial class Cursor : Node2D
{
	[Export] public float Speed = 500f; // pixels per second
	[Export] public Vector2 MinBounds = new Vector2(0, 0);
	[Export] public Vector2 MaxBounds = new Vector2(1920, 1080);

	[Export] public Sprite2D cursor;
	public override void _Process(double delta)
	{
		Vector2 input = Vector2.Zero;

		//Freeze game if diologue is active
		if (GameState.IsDialogueActive)
			return;

		if (Input.IsActionPressed(Global.Controls.MoveUp)) input.Y -= 1;
		if (Input.IsActionPressed(Global.Controls.MoveDown)) input.Y += 1;
		if (Input.IsActionPressed(Global.Controls.MoveLeft)) input.X -= 1;
		if (Input.IsActionPressed(Global.Controls.MoveRight)) input.X += 1;

		if (input != Vector2.Zero)
		{
			input = input.Normalized();
			GlobalPosition += input * Speed * (float)delta;
		}

		GlobalPosition = new Vector2(
			Mathf.Clamp(GlobalPosition.X, MinBounds.X, MaxBounds.X),
			Mathf.Clamp(GlobalPosition.Y, MinBounds.Y, MaxBounds.Y)
		);
	}

	//This is to change the cursor sprite based on context
	public void ShowSelectable()
	{
		cursor.Frame = 1;
	}
	public void ShowNormal()
	{
		cursor.Frame = 0;
	}
	public void ShowSelect()
	{
		cursor.Frame = 2;
	}
}
