using Godot;
using System;

[GlobalClass]
public partial class HealthComponent : BaseComponent
{
    [Export] public float maxHealth = 100;
    private float currentHealth;

    public override void _Ready()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        //GD.Print($"{actor.Name} took {damage} damage.");
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        CallDeferred("DeferredQueueFree");
    }

    public void DeferredQueueFree()
    {
        actor.QueueFree();
    }
}
