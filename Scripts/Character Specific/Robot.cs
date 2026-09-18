using DialogueManagerRuntime;
using Godot;
using System;

public partial class Robot : NPC
{
    //This is the real variable that determines how much time it has left
    [Export] public float timeRemainingFloat = 12f;

    //This variable is purely for the dialogue manager to display correctly
    public int timeRemainingInt;

    [Export] private PackedScene explosionEffect;
    [Export] private DamageZone damageZone;

    public bool hasBlownUp = false;
    public bool defused;

    public override void _Ready()
    {
        base._Ready();

        if (WorldManager.Instance.GetCurrentTime() >= timeRemainingFloat && !defused)
        {
            CallDeferred("DeferQueueFree");
        }
    }

    void DeferQueueFree() => QueueFree();

    public override void _Process(double delta)
    {
        if (!defused)
        {
            RunDownClock();
        }
    }

    private void RunDownClock()
    {
        float currentTime = WorldManager.Instance.GetCurrentTime();

        if (currentTime >= timeRemainingFloat)
        {
            if (!hasBlownUp)
            {
                CallDeferred("BlowUp");
                hasBlownUp = true;
            }
        }

        timeRemainingInt = (int)(timeRemainingFloat - currentTime);
    }

    public void AreaEntered(Area2D area)
    {
        if (area.IsInGroup("FixingArea"))
        {
            CallDeferred("Defuse", area);
        }
    }

    void Defuse(Area2D area)
    {
        defused = true;
        var pickable = GetComponent<Pickable>();
        if (pickable != null)
        {
            pickable.DropDown(true, area.GlobalPosition);
        }
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
