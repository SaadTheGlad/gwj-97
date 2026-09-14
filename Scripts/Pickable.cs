using Godot;
using System;
using System.Net;

[GlobalClass]
public partial class Pickable : Node
{
    //flag for being picked up
    public bool isPickedUp;
    //adjustable array of colliders that disable/enable when being picked up
    [Export] private CollisionShape2D[] colliderShapes;
    //(optional)custom offset when putting it on top of head
    [Export] private float pickUpOffset = 50f;

    public Node2D picker;
    public Node2D parentObject;

    public override void _Ready()
    {
        parentObject = GetParent() as Node2D;
    }

    public void PickUpBy(Node2D _picker)
    {
        isPickedUp = true;
        picker = _picker;

        foreach(CollisionShape2D shape in colliderShapes)
        {
            shape.Disabled = true;
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
            Vector2 lookDir = player.GetLatestLookDirection();
            Ray2D ray = new Ray2D(player.GlobalPosition, lookDir);
            if(parentObject != null)
                parentObject.GlobalPosition = ray.GetPoint(50f) - new Vector2(0, 25f);
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
        }
    }

    public override void _Process(double delta)
    {
        if (isPickedUp)
            parentObject.GlobalPosition = picker.GlobalPosition - new Vector2(0f, pickUpOffset);
    }
}
