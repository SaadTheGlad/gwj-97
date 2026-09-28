using Godot;
using System;

public partial class WorldPortal : Area2D, IComponentable
{
	[Export] private string targetWorldName;
    [Export] private Marker2D spawnPos;

    [Export] private WorldPortal linkedUpPortal;
    [Export] private Sprite2D bg;

    #region Component Related Code
    [Export] private Node componentHolder;
    private BaseComponent[] components;

    //! Put InitComponents and BindComponents() in _Ready() or _EnterTree(), also add the IComponentable interface!

    public override void _Ready()
    {
        InitComponents();
        BindComponents();
    }

    public void InitComponents()
    {
        int componentCount = componentHolder.GetChildCount();

        if (componentCount > 0)
            components = new BaseComponent[componentCount];

        for (int i = 0; i < componentCount; ++i)
        {
            components[i] = componentHolder.GetChild(i) as BaseComponent;
        }
    }

    public void BindComponents()
    {
        if (components == null) return;

        foreach (BaseComponent component in components)
        {
            component.Bind(this);
        }
    }

    public T GetComponent<T>()
    {
        if (components == null) return default(T);

        foreach (BaseComponent component in components)
        {
            if (component is T confirmedComponent)
            {
                return confirmedComponent;
            }
        }

        return default(T);
    }
    #endregion

    private bool playerNear = false;

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
        foreach(var node in GetOverlappingBodies())
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
