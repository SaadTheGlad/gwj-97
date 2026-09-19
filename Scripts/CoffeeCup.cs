using Godot;
using System;

public partial class CoffeeCup : CharacterBodyNPC
{
    [Export] public float speed = 150f;
    Vector2 direction = Vector2.Right;

    Pickable pickableComponent;
    [Export] InteractionArea interactionArea;
    HealthComponent healthComponent;

    public override void _EnterTree()
    {
        base._EnterTree();
        interactionArea.BodyEntered += EnterBody;
    }

    public override void _ExitTree()
    {
        interactionArea.BodyEntered -= EnterBody;
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

    public override void _PhysicsProcess(double delta)
    {
        if (!pickableComponent.isPickedUp)
        {
            Velocity = pickableComponent.dropDownDirection * speed;
            MoveAndSlide();
        }
    }
}
