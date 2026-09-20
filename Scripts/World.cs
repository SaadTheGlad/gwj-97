using Godot;
using System;

[GlobalClass]
public partial class World : Node2D
{
    [Export] private string worldName;
    [Export] private float worldRelativeTimeFactor = 1;
    [Export] private WorldPortal[] portals;

    public string GetWorldName() => worldName;

    //public WorldPortal GetPortal(string worldName)
    //{
    //    foreach(var portal in GetTree().GetNodesInGroup("Portals"))
    //    {
    //        if(portal is WorldPortal worldPortal)
    //        {
    //            if (worldPortal.GetTargetWorldName() == worldName)
    //            {
    //                return worldPortal;
    //            }
    //        }
    //    }

    //    GD.Print($"Could not find portal with name {worldName}");
    //    return null;
    //}



	public float GetRelativeTimeFactor() => worldRelativeTimeFactor;
}
