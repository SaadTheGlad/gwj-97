using Godot;
using System;

public partial class WorldPortal : Node2D
{
	[Export] private string WorldName;

	private bool playerNear = false;

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
        if (Input.IsActionJustPressed("action"))
        {
            CallDeferred("EnterWorld");
        }
    }

    private void EnterWorld() => EventManager.WorldEntered?.Invoke(WorldName, true);

}
