using Godot;
using System;

public partial class WorldPortal : Node2D
{
	[Export] private string WorldName;
    [Export] private Marker2D spawnPos;

    private bool playerNear = false;

    public Vector2 GetSpawnPos()
    {
        return spawnPos.GlobalPosition;
    }

    public string GetWorldName()
    {
        return WorldName;
    }

    public bool IsMarkerThere()
    {
        return spawnPos != null;
    }

    private void BodyEntered(Node2D node)
	{
		if (node.IsInGroup("Player"))
		{
			playerNear = true;
        }
    }

    private void BodyExited(Node2D node)
    {
        if (node.IsInGroup("Player"))
        {
            playerNear = false;
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("action") && playerNear)
        {
            CallDeferred("EnterWorld");
        }
    }

    private void EnterWorld() => EventManager.WorldEntered?.Invoke(WorldName, true);

}
