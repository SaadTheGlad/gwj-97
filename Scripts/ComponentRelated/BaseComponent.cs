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
            GD.PrintErr($"{GetParent().GetPath()} is not GameObject!");
    }
}
