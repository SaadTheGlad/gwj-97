using DialogueManagerRuntime;
using Godot;
using System;

public partial class Robot : NPC
{
    [Export] private float timeInSecondsForSelfDestruct;
    //This variable is purely for the dialogue manager to display correctly
    public int timeRemainingInt;

    [Export] private PackedScene explosionEffect;
    [Export] private DamageZone damageZone;

    bool hasBlownUp = false;
    public bool defused;

    public override void _Ready()
    {
        base._Ready();

        if (WorldManager.Instance.GetCurrentTime() >= timeInSecondsForSelfDestruct)
        {
            CallDeferred("DeferredQueueFree");
        }
    }

    void DeferredQueueFree()
    {
        QueueFree();
    }

    public override void _Process(double delta)
    {
        if(!defused)
            RunDownClock();
    }

    private void RunDownClock()
    {
        float currentTime = WorldManager.Instance.GetCurrentTime();

        if (currentTime >= timeInSecondsForSelfDestruct)
        {
            if (!hasBlownUp)
            {
                CallDeferred("DeferBlowUp");
                hasBlownUp = true;
            }
        }

        timeRemainingInt = (int)(timeInSecondsForSelfDestruct - currentTime);
    }

    public void AreaEntered(Area2D area)
    {
        if (area.IsInGroup("FixingArea"))
        {
            Defuse(area);
        }
    }

    private void Defuse(Area2D area)
    {
        CallDeferred("DeferDefuse", area);
    }

    void DeferDefuse(Area2D area)
    {
        defused = true;
        var pickable = GetComponent<Pickable>();
        if (pickable != null)
        {
            pickable.DropDown(true, area.GlobalPosition);
        }
    }

    private void DeferBlowUp()
    {
        BlowUp();
    }

    private void BlowUp()
    {
        var dialogueComponent = GetComponent<DialogueComponent>();
        if(dialogueComponent != null)
        {
            dialogueComponent.DeleteBalloon();
        }

        //spawning effect
        OneShotParticleEffect particleEffect = explosionEffect.Instantiate() as OneShotParticleEffect;
        particleEffect.GlobalPosition = GlobalPosition;
        GetParent().AddChild(particleEffect);
        particleEffect.EmitThenDestroy();

        //enabling the damage zone
        damageZone.Monitoring = true;
    }
}
