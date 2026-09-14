using Godot;
using System;

public partial class DamageZone : Area2D
{
    [Export] private bool worksWithBodies = true, worksWithAreas = false;


    public void BodyEnteredZone(Node2D body)
    {
        if (worksWithBodies)
        {
            CanTakeDamage canTakeDamage;

            //blow up object in cantake damage
            foreach (Node node in body.GetChildren())
            {
                if (node is CanTakeDamage)
                {
                    canTakeDamage = (CanTakeDamage)node;
                    canTakeDamage.TakeDamage(200f);
                    break;
                }
            }
        }
    }

    public void AreaEnteredZone(Area2D area)
    {
        if (worksWithAreas)
        {
            CanTakeDamage canTakeDamage;

            //blow up object in cantake damage
            //this assumes that the area is the parent ugh...
            foreach (Node node in area.GetChildren())
            {
                if (node is CanTakeDamage)
                {
                    canTakeDamage = (CanTakeDamage)node;
                    canTakeDamage.TakeDamage(200f);
                    break;
                }
            }
        }
    }


}
