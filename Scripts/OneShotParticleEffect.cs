using Godot;
using System;

public partial class OneShotParticleEffect : Node2D
{
    [Export] private GpuParticles2D particles;

    float particlesLifeTime;

    public override void _Ready()
    {
        particlesLifeTime = (float)particles.Lifetime;
    }

    async public void EmitThenDestroy()
    {
        particles.Emitting = true;

        await ToSignal(GetTree().CreateTimer(particlesLifeTime), SceneTreeTimer.SignalName.Timeout);

        QueueFree();
    }
}
