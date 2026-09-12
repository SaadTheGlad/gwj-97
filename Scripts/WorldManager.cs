using Godot;
using System;
using System.Security;

public partial class WorldManager : Node
{
    World currentWorld;
    [Export] private Node2D player;
    [Export] private TimeManager timeManager;

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
        CallDeferred("LoadWorld", "BlueWorld", true);
    }

    void LoadWorld(string levelName, bool useSpawnPos)
    {
        string previousWorldName = "";

        //check if there's a level already loaded, if so, remove it first
        if (currentWorld != null) {
            previousWorldName = currentWorld.Name;
            currentWorld.QueueFree();
        }

        //load the level
        currentWorld = SceneManager.Instance.GetScene(levelName).Instantiate() as World;
        GetParent().AddChild(currentWorld);

        //set the player's position
        if (useSpawnPos)
        {
            Vector2 spawnPos = Vector2.Zero;

            for (int i = 0; i < currentWorld.GetPortals().Length; i++)
            {
                if (currentWorld.GetPortals()[i].GetWorldName() == previousWorldName)
                {
                    player.GlobalPosition = currentWorld.GetPortals()[i].GetSpawnPos();
                } 
            }
        }

        //change time scale
        timeManager.ChangeTimeScale(currentWorld.GetTimeScale());

    }
}
