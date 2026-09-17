using DialogueManagerRuntime;
using Godot;
using System;

[GlobalClass]
public partial class DialogueComponent : BaseComponent
{
    [Export] private Resource dialogueResource;
    protected DialogueBalloon balloon;

    private bool mute;
    public void Mute() => mute = true;
    public void Unmute() => mute = false;


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
        DialogueManager.DialogueEnded -= NullBalloon;
    }

    public void StartDialogue()
    {
        if (!mute)
        {
            balloon = (DialogueBalloon)DialogueManager.ShowDialogueBalloon(dialogueResource, "start");
            //NOT IMPLEMENTED YET
            EventManager.ResetVelocity?.Invoke();
        }
    }

    public void DeleteBalloon()
    {
        if (balloon != null)
        {
            balloon.QueueFree();
        }
    }

    //This function exists so that in the above function we can safely QueueFree();
    private void NullBalloon(Resource endingDialogueResource)
    {
        //This runs twice for some reason
        balloon = null;
    }

}
