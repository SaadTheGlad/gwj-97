using DialogueManagerRuntime;
using Godot;
using System;

public partial class Robot : NPC
{
    [Export] private float totalTimeTillSelfDestruct;
    private float timeRemaining;
    //This variable is purely for the dialogue manager to display correctly
    public int timeRemainingInt;

    public override void _Ready()
    {
        timeRemaining = GameState.Instance.robotTimeRemaining;

        //We check if we haven't sent the game state remaining time first, if not, we do it
        if (timeRemaining == -1)
        {
            timeRemaining = totalTimeTillSelfDestruct;
        }
        else if(timeRemaining <= 0)
        {
            QueueFree();
        }
    }

    public override void _Process(double delta)
    {
        timeRemaining -= (float)delta;
        timeRemaining = Mathf.Clamp(timeRemaining, 0, totalTimeTillSelfDestruct);

        timeRemainingInt = (int)timeRemaining;

        if(timeRemaining <= 0)
        {
            BlowUp();
        }

        GameState.Instance.robotTimeRemaining = timeRemaining;
    }

    private void BlowUp()
    {
        //close speech bubble
        QueueFree();
    }
}
