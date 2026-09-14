using DialogueManagerRuntime;
using Godot;
using System;

public partial class Robot : NPC
{
    [Export] private float timeInSecondsForSelfDestruct;
    //This variable is purely for the dialogue manager to display correctly
    public int timeRemainingInt;

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
    }

    private void BlowUp()
    {
        balloon.QueueFree();
        QueueFree();
    }
}
