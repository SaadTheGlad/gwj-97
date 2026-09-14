using Godot;
using System;

[GlobalClass]
public partial class InteractionArea : Area2D
{
    public Action HasStartedDialogue;
    private Pickable pickable;

    public override void _Ready()
    {
        foreach(Node node in GetParent().GetChildren())
        {
            if(node is Pickable _pickable)
            {
                pickable = _pickable;
                break;
            }
        }
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
