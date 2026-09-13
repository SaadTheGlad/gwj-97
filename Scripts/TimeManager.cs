using Godot;
using System;
using System.Numerics;

public partial class TimeManager : Node
{
    [Export] private float totalTime;
    [Export] private Label timeLabel;
    [Export] private float timeScale = 1;

    private float currentTime;
    private int secondCounter;


    public override void _Ready()
    {
        currentTime = totalTime;
        secondCounter = (int)totalTime;
    }

    public override void _Process(double delta)
    {
        if(currentTime <= 0)
        {
            //restart game.
            GetTree().ReloadCurrentScene();
        }

        if (currentTime <= secondCounter)
        {
            //play audio
            AudioManager.Instance.Play("ticktock");
            //decrement second counter
            secondCounter--;
        }
        currentTime -= (float)delta * timeScale;


        timeLabel.Text = "Time Left: " + currentTime.ToString("0") + "\nTime Scale: " + timeScale.ToString();


    }

    public void ChangeTimeScale(float newTimeScale)
    {
        timeScale = newTimeScale;
    }
}
