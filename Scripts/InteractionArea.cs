using Godot;
using System;

[GlobalClass]
public partial class InteractionArea : Area2D
{
    public Action HasStartedDialogue;
    private Pickable pickable;

    public override void _Ready()
    {
        pickable = GetParent().GetNodeOrNull("Pickable") as Pickable;
    }

    public void StartDialogue()
    {
        HasStartedDialogue?.Invoke();
    }

    public Pickable GetPickable()
    {
        return pickable;
    }
}
