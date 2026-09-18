using Godot;
using System;

public partial class WorldPortal : Area2D, IComponentable
{
	[Export] private string targetWorldName;
    [Export] private Marker2D spawnPos;

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
    }

    private void EnterWorld() => EventManager.WorldEntered?.Invoke(targetWorldName);

}
