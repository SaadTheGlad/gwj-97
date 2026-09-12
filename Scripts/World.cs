using Godot;
using System;

public partial class World : Node2D
{
	[Export] private Marker2D spawnPos;
	
	public Vector2 GetMarkerPos()
	{
		return spawnPos.GlobalPosition;
	}

	public bool IsMarkerThere()
	{
		return spawnPos != null;
	}
}
