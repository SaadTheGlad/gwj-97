using Godot;
using System;

public partial class WorldManager : Node
{
    World currentWorld;
    [Export] private Node2D player;

    public override void _EnterTree()
    {
        EventManager.WorldEntered += LoadWorld;
    }

    public override void _ExitTree()
    {
        EventManager.WorldEntered -= LoadWorld;
    }

    public override void _Ready()
    {
        CallDeferred("LoadWorld", "BlueWorld", false);
    }

    void LoadWorld(string levelName, bool useSpawnPos)
    {
        //check if there's a level already loaded, if so, remove it first
        if(currentWorld != null) currentWorld.QueueFree();

        //load the level
        currentWorld = SceneManager.Instance.GetScene(levelName).Instantiate() as World;
        GetParent().AddChild(currentWorld);

        //set the player's position
        if (useSpawnPos)
        {
            if (currentWorld.IsMarkerThere())
            {
                player.GlobalPosition = currentWorld.GetMarkerPos();
            }
            else
            {
                GD.PushWarning("Spawn marker not set!");
            }
        }

    }
}
