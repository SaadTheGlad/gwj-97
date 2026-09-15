using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] private AnimatedSprite2D animatedSprite;

    //movement and raycast vars
    [Export] private float speed = 15f;
    [Export] private PackedScene testNode;
    Vector2 latestDirection;
    Vector2 direction;
    float maxRayDistance = 30f;

    //dropping variables
    Pickable pickable = null;
    private float droppingGracePeriod = 0.1f;
    private bool canDropObject = true;
    private bool isHoldingSomething;

    //interaction points
    [Export] public Area2D leftPoint, rightPoint, downPoint, topPoint;

    public Vector2 GetLatestLookDirection() => latestDirection;

    public void SetHoldingSomething(bool flag) => isHoldingSomething = flag;

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (GameState.Instance.playerCanMove)
        {
            if (Input.IsActionJustPressed("action"))
            {
                InitiateDialogue();
            }else if (Input.IsActionJustPressed("pick_up") && !isHoldingSomething)
            {
                PickUp();
            }

            direction = Input.GetVector("left", "right", "forward", "backward").Normalized();
        }
    }

    private HitInfo2D SendRaycastReturnHitinfo(uint mask)
    {
        Ray2D ray = new Ray2D(GlobalPosition, latestDirection);
        HitInfo2D hitInfo = new HitInfo2D();

        Raycast.Raycast2D(ray, out hitInfo, GetWorld2D().DirectSpaceState, maxRayDistance, GetRid(), mask);

        #region raycast_debugging
        //Node2D testNode1 = testNode.Instantiate() as Node2D;
        //Node2D testNode2 = testNode.Instantiate() as Node2D;
        //testNode1.GlobalPosition = GlobalPosition;
        //testNode2.GlobalPosition = ray.GetPoint(maxRayDistance);
        //GetTree().Root.AddChild(testNode1);
        //GetTree().Root.AddChild(testNode2);
        #endregion

        return hitInfo;
    }

    public override void _Process(double delta)
    {
        //dropping the object
        if(pickable != null)
        {
            if (pickable.isPickedUp && Input.IsActionJustPressed("pick_up") && canDropObject)
            {
                Drop();
            }
        }
    }

    async private void StartDropGraceTimer()
    {
        await ToSignal(GetTree().CreateTimer(droppingGracePeriod), SceneTreeTimer.SignalName.Timeout);
        canDropObject = true;
    }

    private void SpeakWithOverlappedInteractables(Area2D areaToCheck)
    {
        foreach (Area2D area in areaToCheck.GetOverlappingAreas())
        {
            if (area is InteractionArea interact)
            {
                interact.StartDialogue();
            }

            break;
        }
    }

    private void InitiateDialogue()
    {
        if(latestDirection == Vector2.Left)
        {
            SpeakWithOverlappedInteractables(leftPoint);
        }
        else if(latestDirection == Vector2.Right)
        {
            SpeakWithOverlappedInteractables(rightPoint);
        }
        else if(latestDirection == Vector2.Down)
        {
            SpeakWithOverlappedInteractables(downPoint);
        }
        else if(latestDirection == Vector2.Up)
        {
            SpeakWithOverlappedInteractables(topPoint);
        }

        #region Deprecated Raycast Code
        //uint mask = 1 << 1;
        //HitInfo2D hitInfo = SendRaycastReturnHitinfo(mask);

        //if (hitInfo != null)
        //{
        //    Node node = hitInfo.collider as Node;
        //    if (node is InteractionArea interact)
        //    {
        //        interact.StartDialogue();
        //    }
        //}
        #endregion
    }

    private void PickUpOverlappedInteractables(Area2D areaToPickUp)
    {
        foreach (Area2D area in areaToPickUp.GetOverlappingAreas())
        {
            if (area is InteractionArea interact)
            {
                pickable = interact.GetPickable();
                if (pickable != null)
                {
                    pickable.PickUpBy(this);
                    isHoldingSomething = true;
                    canDropObject = false;
                    StartDropGraceTimer();
                }
            }

            break;
        }
    }

    private void PickUp()
    {
        if (latestDirection == Vector2.Left)
        {
            PickUpOverlappedInteractables(leftPoint);
        }
        else if (latestDirection == Vector2.Right)
        {
            PickUpOverlappedInteractables(rightPoint);
        }
        else if (latestDirection == Vector2.Down)
        {
            PickUpOverlappedInteractables(downPoint);
        }
        else if (latestDirection == Vector2.Up)
        {
            PickUpOverlappedInteractables(topPoint);
        }

        #region Deprecated Raycast Code
        //uint mask = 1 << 1;
        //HitInfo2D hitInfo = SendRaycastReturnHitinfo(mask);

        //if(hitInfo != null)
        //{
        //    Node node = hitInfo.collider as Node;

        //    if(node is InteractionArea interact)
        //    {
        //        pickable = interact.GetPickable();
        //        if(pickable != null)
        //        {
        //            pickable.PickUpBy(this);
        //            isHoldingSomething = true;
        //            canDropObject = false;
        //            StartDropGraceTimer();
        //        }
        //    }
        //}
        #endregion
    }

    private void Drop()
    {
        pickable.DropDown();
        pickable = null;
        isHoldingSomething = false;
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 oldDir = Vector2.Down;

		if (direction == Vector2.Left)
		{
			animatedSprite.Frame = 2;
            latestDirection = direction;
        }
        else if (direction == Vector2.Right) 
		{
			animatedSprite.Frame = 1;
            latestDirection = direction;
        }
        else if (direction == Vector2.Up)
		{
			animatedSprite.Frame = 3;
            latestDirection = direction;
        }
        else if(direction == Vector2.Down)
		{
			animatedSprite.Frame = 0;
            latestDirection = direction;
        }

        Velocity = direction * speed;
		MoveAndSlide();
	}
}
