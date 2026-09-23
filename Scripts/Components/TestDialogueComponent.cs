using DialogueManagerRuntime;
using Godot;
using System;

[GlobalClass]
public partial class TestDialogueComponent : BaseComponent
{
    private Resource dialogueResource = null;

    /// <summary>
    /// The dialogue resource to use when starting dialogue.
    /// </summary>
    [Export]
    public Resource DialogueResource
    {
        get => dialogueResource;
        set
        {
            dialogueResource = value;
            if (dialogueResource == null)
            {
                //DialogueCue = "";
            }
            NotifyPropertyListChanged();
        }
    }

    protected DialogueBalloon balloon;

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
        balloon = (DialogueBalloon)DialogueManager.ShowDialogueBalloon(dialogueResource, "start");     
    }

    public void DeleteBalloon()
    {
        if (balloon != null)
            CallDeferred("DeferQueueFree");       
    }

    void DeferQueueFree()
    {
        balloon.QueueFree();
        balloon = null;
    }

    //This function exists so that in the above function we can safely QueueFree();
    private void NullBalloon(Resource endingDialogueResource)
    {
        GD.Print("Nulled.");

        if(endingDialogueResource == dialogueResource)
            balloon = null;
    }

}
