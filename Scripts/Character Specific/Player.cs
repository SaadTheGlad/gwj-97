using Godot;
using System;
using System.Security;

public partial class Player : CharacterBody2D, IComponentable
{
	[Export] private AnimatedSprite2D animatedSprite;

    //movement vars
    [Export] private float maxSpeed = 15f;
    private float currentSpeed;
    public Vector2 latestDirection;
    Vector2 direction;

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

    public override void _Ready()
    {
        InitComponents();
        BindComponents();

        currentSpeed = maxSpeed;
        latestDirection = Vector2.Down;
    }

    private void InteractBasedOnDirection(Action<Area2D> interactMethod)
    {
        if (latestDirection == Vector2.Left)
        {
            interactMethod(leftPoint);
        }
        else if (latestDirection == Vector2.Right)
        {
            interactMethod(rightPoint);
        }
        else if (latestDirection == Vector2.Down)
        {
            interactMethod(downPoint);
        }
        else if (latestDirection == Vector2.Up)
        {
            interactMethod(topPoint);
        }
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (GameState.Instance.playerCanMove)
        {
            if (Input.IsActionJustPressed("action1"))
            {
                if (direction != Vector2.Zero) return;

                InteractBasedOnDirection(SpeakWithOverlappedInteractables);
            }
            else if (Input.IsActionJustPressed("action2") && !isHoldingSomething)
            {
                InteractBasedOnDirection(PickUpOverlappedInteractables);
            } 
            else if (Input.IsActionJustPressed("action2") && canDropObject)
            {
                DropObject();
            }

            direction = Input.GetVector("left", "right", "forward", "backward").Normalized();
        }
    }

    private void SpeakWithOverlappedInteractables(Area2D areaToCheck)
    {
        foreach (Area2D area in areaToCheck.GetOverlappingAreas())
        {
            if (area is InteractionArea interact)
            {
                Node owner = interact.GetActor();
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

    private void PickUpOverlappedInteractables(Area2D areaToPickUp)
    {
        foreach (Area2D area in areaToPickUp.GetOverlappingAreas())
        {
            if (area is InteractionArea interact)
            {
                Node owner = interact.GetActor();
                if (owner is IComponentable componentable)
                {
                    pickable = componentable.GetComponent<Pickable>();
                    if (pickable != null)
                    {
                        pickable.PickUpBy(this);
                        //This is to make sure that we don't pick up the object then drop it immediately. 
                        canDropObject = false;
                        StartDropGraceTimer();
                    }
                }
            }

            break;
        }
    }

    async private void StartDropGraceTimer()
    {
        await ToSignal(GetTree().CreateTimer(droppingGracePeriod), SceneTreeTimer.SignalName.Timeout);
        canDropObject = true;
    }

    public void DropObject()
    {
        if(pickable != null)
        {
            pickable.DropDown(hasCustomPosition: false, _dropDownDirection: latestDirection);
            pickable = null;
        }
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
