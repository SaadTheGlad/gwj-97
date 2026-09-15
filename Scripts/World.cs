using Godot;
using System;

[GlobalClass]
public partial class World : Node2D
{
    [Export] private float worldTimeScale = 1;
    [Export] private WorldPortal[] portals;

    public WorldPortal[] GetPortals()
    {
        return portals;

    }

	public float GetTimeScale() => worldTimeScale;
}
