using Godot;
using System;

public partial class SlumpedMan : NPC
{
    private int coffeesDrunk;
    private float coffeesDrunkPerMinute;

    private float totalTimeSinceSpawn;

    public void DrinkCoffee()
    {
        coffeesDrunk++;
    }

    public override void _Process(double delta)
    {
        CalculateRateOfCoffeesDrunk(delta);
    }

    public void CalculateRateOfCoffeesDrunk(double delta)
    {
        totalTimeSinceSpawn += (float)delta;
        coffeesDrunkPerMinute = (coffeesDrunk / totalTimeSinceSpawn) * 10f;
        GD.Print($"Rate is: {coffeesDrunkPerMinute} coffees drunk per 10 seconds!");
    }

}
