using DialogueManagerRuntime;
using Godot;

[GlobalClass]
public partial class NPC : StaticBody2D
{
	[Export] private Resource dialogueResource;

	public void StartDialogue()
	{
		DialogueManager.ShowDialogueBalloon(dialogueResource, "start");
    }
}
