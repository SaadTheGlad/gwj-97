using Godot;
using System;

public partial class DamageZone : Area2D
{
    [Export] private bool worksWithBodies = true, worksWithAreas = false;
    [Export] private float damageItDeals = 200f;

    public void BodyEnteredZone(Node2D body)
    {
        if (worksWithBodies)
        {
            if(body is IComponentable componentable)
            {
                var canTakeDamage = componentable.GetComponent<CanTakeDamage>();

                if (canTakeDamage != null)
                {
                    canTakeDamage.TakeDamage(damageItDeals);
                }
            }
        }
    }

    public void AreaEnteredZone(Area2D area)
    {
        if (worksWithAreas)
        {
            if (area is IComponentable componentable)
            {
                var canTakeDamage = componentable.GetComponent<CanTakeDamage>();

                if (canTakeDamage != null)
                {
                    canTakeDamage.TakeDamage(damageItDeals);
                }
            }
        }
    }
}
