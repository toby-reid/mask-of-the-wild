using Godot;
using System;

public partial class Cursor : Node2D
{
	[Export] public float Speed = 500f; // pixels per second
	[Export] public Vector2 MinBounds = new Vector2(0, 0);
	[Export] public Vector2 MaxBounds = new Vector2(960, 720);

	[Export] public Sprite2D cursor;
	public override void _Process(double delta)
	{
		Vector2 input = Vector2.Zero;

		//Freeze game if diologue is active
		if (GameState.IsDialogueActive)
			return;

		if (Input.IsActionPressed(Global.Controls.MoveUp) && GlobalPosition.Y > MinBounds.Y) input.Y -= 1;
		if (Input.IsActionPressed(Global.Controls.MoveDown) && GlobalPosition.Y < MaxBounds.Y) input.Y += 1;
		if (Input.IsActionPressed(Global.Controls.MoveLeft) && GlobalPosition.X > MinBounds.X) input.X -= 1;
		if (Input.IsActionPressed(Global.Controls.MoveRight) && GlobalPosition.X < MaxBounds.X) input.X += 1;

		if (input != Vector2.Zero)
		{
			input = input.Normalized();
			GlobalPosition += input * Speed * (float)delta;
		}
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
