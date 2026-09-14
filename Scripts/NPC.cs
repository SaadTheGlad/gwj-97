using DialogueManagerRuntime;
using Godot;

[GlobalClass]
public partial class NPC : StaticBody2D 
{
	[Export] private Resource dialogueResource;
	[Export] private InteractionArea interactArea;

	protected DialogueBalloon balloon;

    public override void _EnterTree()
    {
		interactArea.HasStartedDialogue += StartDialogue;
    }

    public override void _ExitTree()
    {
        interactArea.HasStartedDialogue -= StartDialogue;
    }

	public void StartDialogue()
	{
		balloon = (DialogueBalloon)DialogueManager.ShowDialogueBalloon(dialogueResource, "start");
    }
}
