using Godot;
using System;

[GlobalClass]
public partial class BaseComponent : Node
{
    protected Node actor;

    public virtual void Bind(Node _actor)
    {
        actor = _actor;
    }

    public virtual void RemoveComponent()
    {
        CallDeferred("DeferredQueueFree");
    }

    void DeferredQueueFree()
    {
        QueueFree();
    }
}
