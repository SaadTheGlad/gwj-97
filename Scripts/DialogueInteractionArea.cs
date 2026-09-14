using Godot;
using System;

public partial class DialogueInteractionArea : Area2D
{
    [Export] private NPC npc;

    public void StartDialogue()
    {
        npc.StartDialogue();
    }
}
