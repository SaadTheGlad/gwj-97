using Godot;
using System;

[GlobalClass]
public partial class BaseComponent : Node
{
    protected GameObject actor;

    public virtual void Bind(Node _actor)
    {
        if (_actor is GameObject gameObject)
            actor = gameObject;
        else
            GD.PrintErr("Actor is not GameObject!");
    }
}
