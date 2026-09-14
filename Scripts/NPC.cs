using DialogueManagerRuntime;
using Godot;

[GlobalClass]
public partial class NPC : StaticBody2D
{
	[Export] private Resource dialogueResource;

	protected DialogueBalloon balloon;

	public void StartDialogue()
	{
		balloon = (DialogueBalloon)DialogueManager.ShowDialogueBalloon(dialogueResource, "start");
    }
}
