using Godot;
using System;

[GlobalClass]
public partial class WorldResource : Resource
{
    [Export] public string worldName;
    [Export] public bool hasLoadedWorld = false;
    [Export] public PackedScene worldPackedScene;
}
