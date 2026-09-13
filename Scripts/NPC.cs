using DialogueManagerRuntime;
using Godot;
using System;

public partial class NPC : StaticBody2D
{
	[Export] private Resource dialogueResource;

	public void StartDialogue()
	{
		DialogueManager.ShowDialogueBalloon(dialogueResource, "start");
    }
}
