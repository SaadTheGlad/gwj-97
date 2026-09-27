using DialogueManagerRuntime;
using Godot;
using System;

public partial class Robot : NPC
{
    //This is the real variable that determines how much time it has left
    [Export] public float totalLifeTime = 12f;

    //This variable is purely for the dialogue manager to display correctly
    private float currentRemainingTime;

    [Export] private PackedScene explosionEffect;
    [Export] private DamageZone damageZone;

    public bool defused;
    float timeScale = 1f;
    void DeferQueueFree() => QueueFree();

    public override void _Ready()
    {
        currentRemainingTime = totalLifeTime;
    }

    public override void _Process(double delta)
    {
        if (!defused)
        {
            RunDownClock(delta);
        }
    }

    private void RunDownClock(double delta)
    {
        if (currentRemainingTime <= 0)
        {
            CallDeferred("BlowUp");         
        }

        if(TryGetComponent<TimeDilationComponent>(out var timeDilationComponent))
        {
            timeScale = timeDilationComponent.GetTimeScale();
        }

        currentRemainingTime -= (float)delta * timeScale;
        GameState.Instance.timeRemainingInt = (int)currentRemainingTime;
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
        if (TryGetComponent<Pickable>(out var pickable))
        {
            pickable.DropDown(true, area.GlobalPosition, Vector2.Zero);
        }

        area.Monitorable = false;

        GameState.Instance.hasDefusedRobot = true;
        AudioManager.Instance.Play("Jingle");

    }

    private void BlowUp()
    {
        if(TryGetComponent<DialogueComponent>(out var dialogueComponent))
        {
            dialogueComponent.DeleteBalloon();
        }

        //spawning effect
        OneShotParticleEffect particleEffect = explosionEffect.Instantiate() as OneShotParticleEffect;
        particleEffect.GlobalPosition = GlobalPosition;
        GetTree().Root.AddChild(particleEffect);
        particleEffect.EmitThenDestroy();

        //enabling the damage zone
        damageZone.Monitoring = true;
    }
}
