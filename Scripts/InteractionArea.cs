using Godot;
using System;

[GlobalClass]
public partial class InteractionArea : Area2D
{
    public Action HasStartedDialogue;
    private Node owner;

    public override void _Ready()
    {
        owner = GetParent();
    }

    //this is so we can see if it has any relevant components
    public Node GetOwnerNode() => owner;

    public void StartDialogue()
    {
        HasStartedDialogue?.Invoke();
    }
}
