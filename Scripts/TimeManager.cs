using Godot;
using System;

public partial class TimeManager : Node
{
    [Export] private float totalTime = 60f;
    [Export] private Label timeLabel;
    [Export] private float timeScale = 1;

    private float currentTime;

    public override void _Ready()
    {
        currentTime = totalTime;
    }

    public override void _Process(double delta)
    {
        currentTime -= (float)delta * timeScale;
        timeLabel.Text = "Time Left: " + currentTime.ToString("0") + "\nTime Scale: " + timeScale.ToString();
    }

    public void ChangeTimeScale(float newTimeScale)
    {
        timeScale = newTimeScale;
    }
}
