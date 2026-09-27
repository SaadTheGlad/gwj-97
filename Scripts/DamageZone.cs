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
        //we need to get the parent cuz the body will always be a child of gameobject
        if (detectedNode.GetParent() is GameObject gameObject)
        {
            if (gameObject.TryGetComponent<HealthComponent>(out var healthComponent))
            {
                healthComponent.TakeDamage(damage);
            }
        }
    }
}
