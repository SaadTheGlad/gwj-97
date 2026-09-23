using Godot;
using System;
using System.Security;

public partial class TestPlayer : CharacterBody2D
{
    [Export] private AnimatedSprite2D animatedSprite;

    //movement vars
    [Export] private float speed = 300f;
    public Vector2 latestDirection;
    Vector2 direction;

    //interaction points
    [Export] public Area2D leftPoint, rightPoint, downPoint, topPoint;

    public Vector2 GetLatestLookDirection() => latestDirection;

    public override void _Ready()
    {
        //GetTree().Paused = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (Input.IsActionJustPressed("action1"))
        {
            InitiateDialogue();
        }

        direction = Input.GetVector("left", "right", "forward", "backward").Normalized();
        
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
                    var dialogueComponent = componentable.GetComponent<TestDialogueComponent>();
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
        if (latestDirection == Vector2.Left)
        {
            SpeakWithOverlappedInteractables(leftPoint);
        }
        else if (latestDirection == Vector2.Right)
        {
            SpeakWithOverlappedInteractables(rightPoint);
        }
        else if (latestDirection == Vector2.Down)
        {
            SpeakWithOverlappedInteractables(downPoint);
        }
        else if (latestDirection == Vector2.Up)
        {
            SpeakWithOverlappedInteractables(topPoint);
        }
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
        else if (direction == Vector2.Down)
        {
            animatedSprite.Frame = 0;
            latestDirection = direction;
        }

        Velocity = direction * speed;
        MoveAndSlide();
    }
}
