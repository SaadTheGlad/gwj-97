using Godot;
using System;

public partial class CoffeeMachine : NPC
{
    [Export] AnimatedSprite2D animatedSprite;
    Pickable pickableComponent;
    private Vector2 lookDirection;

    //coffee variables
    [Export] public PackedScene coffeeScene;
    [Export] public float spawnInterval = 1f;
    float tillNextCoffee = 0f;
    TimeDilationComponent timeDilationComponent;
    [Export] private Marker2D left, right, bottom, top;


    public void SetLookDirection(Vector2 _lookDirection) => lookDirection = _lookDirection;

    public override void _Ready()
    {
        pickableComponent = GetComponent<Pickable>();
        timeDilationComponent = GetComponent<TimeDilationComponent>();

        SetLookDirection(Vector2.Down);
    }

    public override void _Process(double delta)
    {
        if (pickableComponent != null)
        {
            if (pickableComponent.isPickedUp)
            {
                if (pickableComponent.picker is Player playerPicker)
                {
                    SetLookDirection(playerPicker.latestDirection);
                }
            }
        }

        if (lookDirection == Vector2.Down)
        {
            animatedSprite.Frame = 0;
        }else if(lookDirection == Vector2.Right || lookDirection == Vector2.Left)
        {
            animatedSprite.Frame = 2;
        }else if(lookDirection == Vector2.Up)
        {
            animatedSprite.Frame = 1;
        }

        if(!pickableComponent.isPickedUp)
            SpawnCoffeeCups(delta);
    }

    public void SpawnCoffeeCups(double delta)
    {
        if (timeDilationComponent.objectInSameWorldAsPlayer)
        {
            tillNextCoffee += (float)delta;
        }
        else
        {
            tillNextCoffee += (float)delta * timeDilationComponent.GetTimeScale();
        }

        if (tillNextCoffee >= spawnInterval)
        {
            tillNextCoffee = 0f;
            CoffeeCup coffeeCup = coffeeScene.Instantiate() as CoffeeCup;
            GetParent().AddChild(coffeeCup);

            if (lookDirection == Vector2.Down)
            {
                coffeeCup.GlobalPosition = bottom.GlobalPosition;
            }
            else if (lookDirection == Vector2.Right)
            {
                coffeeCup.GlobalPosition = right.GlobalPosition;

            }
            else if (lookDirection == Vector2.Left)
            {
                coffeeCup.GlobalPosition = left.GlobalPosition;

            }
            else if (lookDirection == Vector2.Up)
            {
                coffeeCup.GlobalPosition = top.GlobalPosition;

            }

            coffeeCup.SetMovementDirection(lookDirection);

        }
    }
}
