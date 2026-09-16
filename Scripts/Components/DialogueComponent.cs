using DialogueManagerRuntime;
using Godot;
using System;

[GlobalClass]
public partial class DialogueComponent : BaseComponent
{
    [Export] private Resource dialogueResource;
    protected DialogueBalloon balloon;

    private InteractionArea interactArea;

    public DialogueBalloon GetBalloon()
    {
        return balloon;
    }

    public override void _EnterTree()
    {
        DialogueManager.DialogueEnded += NullBalloon;
    }

    public override void _ExitTree()
    {
        interactArea.HasStartedDialogue -= StartDialogue;
        DialogueManager.DialogueEnded -= NullBalloon;
    }

    public void BindInteractArea(InteractionArea _interactArea)
    { 
        interactArea = _interactArea;
        interactArea.HasStartedDialogue += StartDialogue;
    }

    public void StartDialogue()
    {
        balloon = (DialogueBalloon)DialogueManager.ShowDialogueBalloon(dialogueResource, "start");
        EventManager.ResetVelocity?.Invoke();
    }
    private void NullBalloon(Resource endingDialogueResource)
    {
        //This runs twice for some reason
        balloon = null;
    }

}
