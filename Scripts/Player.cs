using Godot;
using System;
using System.Security;

public partial class Player : CharacterBody2D, IComponentable
{
	[Export] private AnimatedSprite2D animatedSprite;

    //movement and raycast vars
    [Export] private float maxSpeed = 15f;
    private float currentSpeed;
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

    #region Component Related Code
    [Export] private Node componentHolder;
    private BaseComponent[] components;

    //! Put InitComponents in _Ready() or _EnterTree() and add IComponentable interface!

    public void InitComponents()
    {
        int componentCount = componentHolder.GetChildCount();

        if (componentCount > 0)
            components = new BaseComponent[componentCount];

        for (int i = 0; i < componentCount; ++i)
        {
            components[i] = componentHolder.GetChild(i) as BaseComponent;
        }
    }

    public void BindComponents()
    {
        if (components == null) return;

        foreach (BaseComponent component in components)
        {
            component.Bind(this);
        }
    }

    public T GetComponent<T>()
    {
        if (components == null) return default(T);

        foreach (BaseComponent component in components)
        {
            if (component is T confirmedComponent)
            {
                return confirmedComponent;
            }
        }

        return default(T);
    }
    #endregion


    public override void _EnterTree()
    {
        EventManager.ResetVelocity += ResetDirection;
    }

    public override void _ExitTree()
    {
        EventManager.ResetVelocity -= ResetDirection;
    }

    public override void _Ready()
    {
        currentSpeed = maxSpeed;
        InitComponents();
        BindComponents();
    }

    void ResetDirection()
    {
        //do stuff and things
    }

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
                Node owner = interact.GetOwnerNode();
                if (owner is IComponentable componentable)
                {
                    var dialogueComponent = componentable.GetComponent<DialogueComponent>();
                    if (dialogueComponent != null)
                    {
                        dialogueComponent.StartDialogue();
                    }
                }
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
                Node owner = interact.GetOwnerNode();
                if(owner is IComponentable componentable)
                {
                    pickable = componentable.GetComponent<Pickable>();
                    if (pickable != null)
                    {
                        pickable.PickUpBy(this);
                        canDropObject = false;
                        StartDropGraceTimer();
                    }
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
    }

    public void ApplyMovementPenalty(float movementPenalty)
    {
        currentSpeed = currentSpeed - currentSpeed * (movementPenalty / 100f);
    }

    public void RemoveMovementPenalty()
    {
        currentSpeed = maxSpeed;
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

        Velocity = direction * currentSpeed;
		MoveAndSlide();
	}
}
