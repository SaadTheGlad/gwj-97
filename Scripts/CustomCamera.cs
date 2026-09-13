using Godot;
using System;

public partial class CustomCamera : Camera2D
{
	[Export] public Node2D player;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GlobalPosition = player.GlobalPosition;
	}
}
