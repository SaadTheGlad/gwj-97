using Godot;
using System;
using System.Net;
using System.Runtime.InteropServices;

[GlobalClass]
public partial class Pickable : BaseComponent
{
    public bool isPickedUp;

    //adjustable array of colliders that disable/enable when being picked up
    [Export] private CollisionShape2D[] colliderShapes;

    //(optional)custom offset when putting it on top of head
    [Export] private float pickUpOffset = 50f;

    //movement penalty in percentage for picking up
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

        //applying movement penalty and setting player flag that it's holding something
        if(picker is Player player)
        {
            //Usually I'd move these to a PickerComponent but it's fine for now
            player.SetHoldingSomething(true);
            player.ApplyMovementPenalty(movementPenalty);

            //Sets the initial position
            if (actor is Node2D actor2D)
            {
                actor2D.GlobalPosition = picker.GlobalPosition - new Vector2(0f, pickUpOffset);
            }
        }


        IComponentable componentable = actor as IComponentable;

        //changing the parent of the object to be the picker's child
        actor.Reparent(picker);

        //Stop being able to talk with this object while it's picked up if it has a dialogue component
        var dialogueComponent = componentable.GetComponent<DialogueComponent>();
        if(dialogueComponent != null)
        {
            dialogueComponent.Mute();
        }

    }

    public void DropDown(bool hasCustomPosition, [Optional] Vector2 customDropPosition)
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
            {
                if (hasCustomPosition)
                {
                    actor2D.GlobalPosition = customDropPosition;
                }
                else
                {
                    actor2D.GlobalPosition = ray.GetPoint(50f) - new Vector2(0, 25f);
                }
            }
        }

        IComponentable componentable = actor as IComponentable;

        //change its parent to be the current world
        World currentWorld = WorldManager.Instance.GetCurrentWorldForPlayer();

        if (actor.GetParent() != currentWorld)
            actor.Reparent(currentWorld);

        //makes you able to talk to the object
        var dialogueComponent = componentable.GetComponent<DialogueComponent>();
        if (dialogueComponent != null)
        {
            dialogueComponent.Unmute();
        }

        //TODO: Should probably check if there is a collider here so you can't place it inside colliders

        //Sets picker to null to avoid headaches.
        picker = null;

    }
}
