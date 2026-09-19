using Godot;
using System;

public partial class CoffeeCup : CharacterBodyNPC
{
    [Export] public float speed = 150f;
    Vector2 direction = Vector2.Right;

    Pickable pickableComponent;

    public override void _Ready()
    {
        pickableComponent = GetComponent<Pickable>();
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
