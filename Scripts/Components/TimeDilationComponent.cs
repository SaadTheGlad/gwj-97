using Godot;
using System;

[GlobalClass]
public partial class TimeDilationComponent : BaseComponent
{
    protected float timeScale = 1f;
    public float GetTimeScale() => timeScale;

    public bool objectInSameWorldAsPlayer;

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
            objectInSameWorldAsPlayer = true;
            return;
        }

        World actorCurrentWorld = null;
        if (actor is CoffeeCup coffee)
        {
            actorCurrentWorld = WorldManager.Instance.GetWorld(coffee.GetCurrentWorldName());
        }
        else
        {
            //Here we assume that current world is always the one that the player is in.
            actorCurrentWorld = actor.GetParent() as World;
        }

        if (WorldManager.Instance.GetCurrentWorldForPlayer().GetWorldName() != actorCurrentWorld.GetWorldName())
        {
            float playerRelativeFactor = WorldManager.Instance.GetCurrentWorldForPlayer().GetRelativeTimeFactor();
            float actorRelativeFactor = actorCurrentWorld.GetRelativeTimeFactor();
            timeScale = actorRelativeFactor / playerRelativeFactor;

            //GD.Print($"Current time scale of {actor.Name} is {timeScale}");

            objectInSameWorldAsPlayer = false;
        }
        else
        {

            objectInSameWorldAsPlayer = true;
            timeScale = 1f;
        }
        
    }
}
