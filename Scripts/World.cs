using Godot;
using System;

[GlobalClass]
public partial class World : Node2D
{
    [Export] private string worldName;
    [Export] private float worldRelativeTimeFactor = 1;
    [Export] private WorldPortal[] portals;

    public string GetWorldName() => worldName;

    public WorldPortal GetPortal(string worldName)
    {
        foreach(var portal in portals)
        {
            if (portal.GetTargetWorldName() == worldName)
            {
                return portal;
            }
        }

        GD.Print("Could not find portal");
        return null;
    }



	public float GetRelativeTimeFactor() => worldRelativeTimeFactor;
}
