using Godot;
using Godot.Collections;
using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using static System.Formats.Asn1.AsnWriter;

public partial class WorldManager : Node
{
    //Singleton
    public static WorldManager Instance { get; private set; }

    //World related variables
    [Export] private World[] worlds;
    World currentWorld;
    [Export] private Node2D player;

    //Time related variables
    [Export] private float timeLoopTime;
    [Export] private Label timeLabel;
    private float currentTimeLeft;
    private int secondCounter;

    public override void _EnterTree()
    {
        if (Instance != null)
        {
            GD.Print("More than one ", Instance.Name);
        }
        else
        {
            Instance = this;
        }

        EventManager.WorldEntered += ChangeWorld;
        EventManager.GameOver += RestartGame;
    }

    public override void _ExitTree()
    {
        EventManager.WorldEntered -= ChangeWorld;
        EventManager.GameOver -= RestartGame;
    }

    public override void _Ready()
    {
        //loads starting world
        currentTimeLeft = timeLoopTime;
        secondCounter = (int)timeLoopTime;

        foreach(var world in worlds)
        {
            if(world.GetWorldName() == "BlueWorld")
            {
                currentWorld = world;
                break;
            }
        }
    }


    public override void _Process(double delta)
    {
        if (currentTimeLeft <= 0)
        {
            RestartGame();
        }

        VisualizeTime(delta);
    }

    public World GetCurrentWorldForPlayer()
    {
        return currentWorld;
    }

    public void VisualizeTime(double delta)
    {
        if (currentTimeLeft <= secondCounter)
        {
            AudioManager.Instance.Play("ticktock");
            secondCounter--;
        }
        currentTimeLeft -= (float)delta;
        timeLabel.Text = "DEBUG\nTime Left Relative to you: " + currentTimeLeft.ToString("0") + "\nRelative Time Scale: " + currentWorld.GetTimeScale().ToString();
    }

    public float GetCurrentTime()
    {
        return timeLoopTime - currentTimeLeft;
    }

    private void RestartGame()
    {
        //I just set it to null here cuz if I don't it breaks
        Instance = null;
        GetTree().ReloadCurrentScene();
    }

    void ChangeWorld(string targetWorldName)
    {
        //change position
        World targetWorld = null;
        foreach(World world in worlds)
        {
            if(world.GetWorldName() == targetWorldName)
            {
                targetWorld = world;
                break;
            }
        }

        WorldPortal targetPortal = targetWorld.GetPortal(currentWorld.GetWorldName());
        player.GlobalPosition = targetPortal.GetSpawnPos();

        currentWorld = targetWorld;

        EventManager.ChangedWorld?.Invoke();
    }
}
