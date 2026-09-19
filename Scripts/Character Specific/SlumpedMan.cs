using Godot;
using System;

public partial class SlumpedMan : NPC
{
    private int coffeesDrunk;
    private float coffeesDrunkPerSecond;

    private float totalTimeSinceSpawn;

    public void DrinkCoffee()
    {
        coffeesDrunk++;
    }

    public override void _Process(double delta)
    {
        CalculateRateOfCoffeesDrunk(delta);
    }

    bool satiated = false;

    public void CalculateRateOfCoffeesDrunk(double delta)
    {
        totalTimeSinceSpawn += (float)delta;
        coffeesDrunkPerSecond = (coffeesDrunk / totalTimeSinceSpawn);
        //GD.Print($"Rate is: {coffeesDrunkPerSecond} coffees drunk per second!");
        if(coffeesDrunkPerSecond >= 2f && !satiated)
        {
            GD.Print("satiated");
            AudioManager.Instance.Play("Jingle");
            GameState.Instance.hasSatiatedSlumpedMan = true;
            satiated = true;
        }
    }

}
