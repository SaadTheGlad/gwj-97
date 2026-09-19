using Godot;
using System;

[GlobalClass]
public partial class TimeDilationComponent : BaseComponent
{
    protected float timeScale = 1f;
    public float GetTimeScale() => timeScale;

    public override void _EnterTree()
    {
        EventManager.ChangedWorld += CheckIfActorIsInSameWorldAsPlayer;
    }

    public override void _ExitTree()
    {
        EventManager.ChangedWorld -= CheckIfActorIsInSameWorldAsPlayer;
    }

    public override void _Ready()
    {
        CheckIfActorIsInSameWorldAsPlayer();
    }

    public void CheckIfActorIsInSameWorldAsPlayer()
    {
        if(actor.GetParent() is Player)
        {
            //we know the timescale should be 1 anyways
            timeScale = 1;
            return;
        }

        //Here we assume that current world is always the one that the player is in.
        var actorCurrentWorld = actor.GetParent() as World;
        if (WorldManager.Instance.GetCurrentWorldForPlayer().GetWorldName() != actorCurrentWorld.GetWorldName())
        {
            float playerRelativeFactor = WorldManager.Instance.GetCurrentWorldForPlayer().GetRelativeTimeFactor();
            float actorRelativeFactor = actorCurrentWorld.GetRelativeTimeFactor();
            timeScale = actorRelativeFactor / playerRelativeFactor;

            GD.Print($"Current time scale of {actor.Name} is {timeScale}");
        }
        else
        {
            timeScale = 1f;
        }
        
    }
}
