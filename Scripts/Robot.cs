using DialogueManagerRuntime;
using Godot;
using System;

public partial class Robot : NPC, IPickable
{
    [Export] private float timeInSecondsForSelfDestruct;
    [Export] private CollisionShape2D hardCollider, interactCollider;
    //This variable is purely for the dialogue manager to display correctly
    public int timeRemainingInt;

    bool isBeingPickedup;
    Node2D theNodeThatPickedYouUp = null;


    public override void _Ready()
    {
        if (WorldManager.Instance.GetCurrentTime() >= timeInSecondsForSelfDestruct)
        {
            QueueFree();
        }
    }

    public override void _Process(double delta)
    {
        float currentTime = WorldManager.Instance.GetCurrentTime();

        if (currentTime >= timeInSecondsForSelfDestruct)
        {
            BlowUp();
        }

        timeRemainingInt = (int)(timeInSecondsForSelfDestruct - currentTime);

        if (isBeingPickedup)
        {
            GlobalPosition = theNodeThatPickedYouUp.GlobalPosition - new Vector2(0, 50f);
            hardCollider.Disabled = true;
            interactCollider.Disabled = true;
        }
    }

    public void PickUp(Node2D _theNodeThatPickedYouUp)
    {
        isBeingPickedup = true;
        theNodeThatPickedYouUp = _theNodeThatPickedYouUp;
    }


    private void BlowUp()
    {
        if(balloon != null)
            balloon.QueueFree();
        QueueFree();
    }
}
