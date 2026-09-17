using Godot;
using System;

[GlobalClass]
public partial class InteractionArea : Area2D
{
    private Node actor;

    public override void _Ready()
    {
        actor = GetParent();
    }

    //this is so we can see if it has any relevant components
    public Node GetActor() => actor;
}
