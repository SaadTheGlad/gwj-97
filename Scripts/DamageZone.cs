using Godot;
using System;

public partial class DamageZone : Area2D
{
    [Export] private bool worksWithBodies = true, worksWithAreas = false;
    [Export] private float damage = 200f;

    public void BodyEnteredZone(Node2D body)
    {
        if (worksWithBodies)
        {
            DealDamage(body);
        }
    }

    public void AreaEnteredZone(Area2D area)
    {
        if (worksWithAreas)
        {
            DealDamage(area);
        }
    }

    private void DealDamage(Node detectedNode)
    {
        if (detectedNode is IComponentable componentable)
        {
            var canTakeDamage = componentable.GetComponent<HealthComponent>();
            if (canTakeDamage != null)
            {
                canTakeDamage.TakeDamage(damage);
            }
        }
    }
}
