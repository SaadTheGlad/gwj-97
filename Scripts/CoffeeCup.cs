using Godot;
using System;

public partial class CoffeeCup : NPC
{
    [Export] public float speed = 150f;
    Vector2 direction = Vector2.Right;

    Pickable pickableComponent;
    HealthComponent healthComponent;
    [Export] CharacterBody2D body;

    Vector2 movementDirection;
    public void SetMovementDirection(Vector2 _movementDirection) => movementDirection = _movementDirection;

    public bool canMove = true;

    public override void _EnterTree()
    {
        base._EnterTree();
        interactArea.BodyEntered += EnterBody;
        interactArea.AreaEntered += EnterArea;
    }

    public override void _ExitTree()
    {
        interactArea.BodyEntered -= EnterBody;
        interactArea.AreaEntered -= EnterArea;

    }

    public override void _Ready()
    {
        pickableComponent = GetComponent<Pickable>();
        healthComponent = GetComponent<HealthComponent>();
    }

    public void EnterBody(Node2D body)
    {
        if (body.IsInGroup("Obstacles"))
        {
            healthComponent.TakeDamage(1000f);
        }

    }

    public void EnterArea(Area2D area)
    {
        if (area is InteractionArea interactArea)
        {
            GD.Print(interactArea.GetOwner().Name);

            if(interactArea.GetActor() is SlumpedMan slumpedMan)
            {
                slumpedMan.DrinkCoffee();
                QueueFree();
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if(!pickableComponent.isPickedUp)
        {
            if(canMove)
                body.Velocity = movementDirection * speed;
        }
        else
        {
            body.Velocity = Vector2.Zero;
            canMove = false;
        }
        body.MoveAndSlide();
    }
}
