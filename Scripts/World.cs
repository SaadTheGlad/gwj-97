using Godot;
using System;

[GlobalClass]
public partial class World : Node2D
{
    [Export] private float timeScale = 1;
    [Export] private WorldPortal[] portals;

    public WorldPortal[] GetPortals()
    {
        return portals;
    }

	public float GetTimeScale() => timeScale;
}
