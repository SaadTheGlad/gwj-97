using Godot;
using System;

public partial class SlumpedMan : NPC
{
    private int coffeesDrunk;
    private float timer = 0f;

    public void DrinkCoffee()
    {
        coffeesDrunk++;
    }

    public override void _Process(double delta)
    {
        if(!satiated)
            CalculateRateOfCoffeesDrunk(delta);
    }

    
    bool satiated = false;
    public void CalculateRateOfCoffeesDrunk(double delta)
    {
        timer += (float)delta;
        if (timer >= 10f)
        {
            //check coffees in last 10 seconds
            if(coffeesDrunk >= 20f)
            {
                AudioManager.Instance.Play("Jingle");
                GameState.Instance.hasSatiatedSlumpedMan = true;
                satiated = true;
            }
            else
            {
                GD.Print($"Coffees drunk in past 10 seconds: {coffeesDrunk}");
                coffeesDrunk = 0;
                timer = 0f;
            }
        }
    }

    //public void CalculateRateOfCoffeesDrunk(double delta)
    //{
    //    totalTimeSinceSpawn += (float)delta;
    //    coffeesDrunkPerSecond = (coffeesDrunk / totalTimeSinceSpawn);
    //    //GD.Print($"Rate is: {coffeesDrunkPerSecond} coffees drunk per second!");
    //    if(coffeesDrunkPerSecond >= 2f && !satiated)
    //    {
    //        AudioManager.Instance.Play("Jingle");
    //        GameState.Instance.hasSatiatedSlumpedMan = true;
    //        satiated = true;
    //    }
    //}

}
