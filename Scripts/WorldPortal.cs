using Godot;
using System;

public partial class WorldPortal : NPC
{
	[Export] private string targetWorldName;
    [Export] private Marker2D spawnPos;

    [Export] private WorldPortal linkedUpPortal;
    [Export] private Sprite2D bg;

    private bool playerNear = false;

    public override void _EnterTree()
    {
        base._EnterTree();
        interactArea.BodyEntered += EnteredBody;
        interactArea.BodyExited += ExitedBody;

    }

    public override void _ExitTree()
    {
        base._ExitTree();
        interactArea.BodyEntered -= EnteredBody;
        interactArea.BodyExited -= ExitedBody;
    }

    public Vector2 GetSpawnPos()
    {
        return spawnPos.GlobalPosition;
    }

    public string GetTargetWorldName()
    {
        return targetWorldName;
    }

    public bool IsMarkerThere()
    {
        return spawnPos != null;
    }

    private void EnteredBody(Node2D node)
	{
		if (node.IsInGroup("Player"))
		{
			playerNear = true;
        }

        World currentWorld = GetParent() as World;
        string currentWorldName = currentWorld.GetWorldName();

        PortalableComponent portalableComponent = null;

        GameObject gameObject = null;
        if(node.GetParent() is GameObject _gameObject)
        {
            gameObject = _gameObject;

            if(gameObject.TryGetComponent<PortalableComponent>(out var _portalableComponent))
            {
                portalableComponent = _portalableComponent;
            }
        }

        if (gameObject == null) return;

        if (gameObject.IsInGroup("GoesThroughPortals") && portalableComponent.canTeleport)
        {
            portalableComponent.SetTimeOut();

            //change position
            World targetWorld = null;
            foreach (World world in WorldManager.Instance.worlds)
            {
                if (world.GetWorldName() == targetWorldName)
                {
                    targetWorld = world;
                    break;
                }
            }

            //When changing the world of the object we change its world too
            if(gameObject is NPC npc)
            {
                npc.SettingCurrentWorldName(targetWorld.GetWorldName());
            }

            WorldPortal targetPortal = linkedUpPortal;

            node.GlobalPosition = targetPortal.GetSpawnPos();
            //we need to reparent for the time dilation component to work
            CallDeferred("Reparent", gameObject, targetWorld);
        }
    }

    private void Reparent(Node toBeParented, Node newParent)
    {
        toBeParented.Reparent(newParent, keepGlobalTransform: true);

        if(toBeParented is GameObject gameObject)
        {
            if(gameObject.TryGetComponent<TimeDilationComponent>(out var timeDilationComponent))
            {
                timeDilationComponent.CheckIfActorIsInSameWorldAsPlayer();
            }
            else
            {
                GD.Print("No time dilation component");
            }
        }
    }

    private void ExitedBody(Node2D node)
    {
        if (node.IsInGroup("Player"))
        {
            playerNear = false;
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("action1") && playerNear)
        {
            CallDeferred("EnterWorld");
        }

        ((ShaderMaterial)bg.Material).SetShaderParameter("enabled", CheckForOutline());
    }

    bool CheckForOutline()
    {
        foreach(var node in interactArea.GetOverlappingBodies())
        {
            if(node is Player)
            {
                return true;
            }
        }
    
        return false;
    }   

    private void EnterWorld()
    {
        EventManager.WorldEntered?.Invoke(targetWorldName, linkedUpPortal);
    }
}
