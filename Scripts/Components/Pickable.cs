using Godot;
using System;
using System.Net;

[GlobalClass]
public partial class Pickable : BaseComponent
{
    //flag for being picked up
    public bool isPickedUp;
    //adjustable array of colliders that disable/enable when being picked up
    [Export] private CollisionShape2D[] colliderShapes;
    //(optional)custom offset when putting it on top of head
    [Export] private float pickUpOffset = 50f;

    //movement penalty for picking up
    [Export(PropertyHint.Range, "0, 100, 1")] private float movementPenalty = 0f;

    public Node2D picker;

    public void PickUpBy(Node2D _picker)
    {
        isPickedUp = true;
        picker = _picker;

        foreach(CollisionShape2D shape in colliderShapes)
        {
            shape.Disabled = true;
        }

        //applying movement penalty
        if(picker is Player player)
        {
            player.SetHoldingSomething(true);
            player.ApplyMovementPenalty(movementPenalty);
        }
    }

    public void DropDown()
    {
        isPickedUp = false;

        foreach (CollisionShape2D shape in colliderShapes)
        {
            shape.Disabled = false;
        }
        
        //set its position to something sensible
        if(picker is Player player)
        {
            player.SetHoldingSomething(false);
            player.RemoveMovementPenalty();
            Vector2 lookDir = player.GetLatestLookDirection();
            Ray2D ray = new Ray2D(player.GlobalPosition, lookDir);
            if(actor is Node2D actor2D)
                actor2D.GlobalPosition = ray.GetPoint(50f) - new Vector2(0, 25f);
        }

        //should probably check if there is a collider here so you can't place it inside colliders

        picker = null;

    }

    public void DropDown(Vector2 customDropPosition)
    {
        isPickedUp = false;

        foreach (CollisionShape2D shape in colliderShapes)
        {
            shape.Disabled = false;
        }

        //set its position to something sensible
        if (picker is Player player)
        {
            player.SetHoldingSomething(false);
            player.RemoveMovementPenalty();
            Vector2 lookDir = player.GetLatestLookDirection();
            Ray2D ray = new Ray2D(player.GlobalPosition, lookDir);
            if (actor is Node2D actor2D)
                actor2D.GlobalPosition = customDropPosition;
        }

        //should probably check if there is a collider here so you can't place it inside colliders

        picker = null;

    }

    public override void _ExitTree()
    {
        //this is to ensure that the player flag isn't still set
        if(picker is Player player)
        {
            player.SetHoldingSomething(false);
            player.RemoveMovementPenalty();
        }
    }

    public override void _Process(double delta)
    {
        if (isPickedUp && actor is Node2D actor2D)
            actor2D.GlobalPosition = picker.GlobalPosition - new Vector2(0f, pickUpOffset);
    }
}
