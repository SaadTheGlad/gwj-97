using Godot;
using System;

public partial class InteractionArea : Area2D
{
    public Action HasStartedDialogue;

    public void StartDialogue()
    {
        HasStartedDialogue?.Invoke();
    }
}
