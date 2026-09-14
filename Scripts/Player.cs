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

    public Vector2 GetLatestLookDirection() => latestDirection;

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

    private void InitiateDialogue()
    {
        uint mask = 1 << 1;
        HitInfo2D hitInfo = SendRaycastReturnHitinfo(mask);

        if (hitInfo != null)
        {
            Node node = hitInfo.collider as Node;
            if (node is InteractionArea interact)
            {
                interact.StartDialogue();
            }
        }
    }

    private void PickUp()
    {
        uint mask = 1 << 1;
        HitInfo2D hitInfo = SendRaycastReturnHitinfo(mask);

        if(hitInfo != null)
        {
            Node node = hitInfo.collider as Node;
            if(node is InteractionArea interact)
            {
                pickable = interact.GetPickable();
                if(pickable != null)
                {
                    pickable.PickUpBy(this);
                    isHoldingSomething = true;
                    canDropObject = false;
                    StartDropGraceTimer();
                }
            }
        }
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
