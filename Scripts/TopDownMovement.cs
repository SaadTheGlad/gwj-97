using Godot;
using System;

public partial class TopDownMovement : CharacterBody2D
{
	[Export] private float speed = 15f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("left", "right", "forward", "backward").Normalized();

		Velocity = direction * speed;

		MoveAndSlide();

	}
}
