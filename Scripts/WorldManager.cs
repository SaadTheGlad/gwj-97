using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Security;

public partial class WorldManager : Node
{
    //Singleton
    public static WorldManager Instance { get; private set; }

    //World related variables
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

        EventManager.WorldEntered += LoadWorld;
    }

    public override void _ExitTree()
    {
        EventManager.WorldEntered -= LoadWorld;
    }

    public override void _Ready()
    {
        CallDeferred("LoadWorld", "BlueWorld", true);

        currentTimeLeft = timeLoopTime;
        secondCounter = (int)timeLoopTime;
    }

    public float GetCurrentTime()
    {
        return timeLoopTime - currentTimeLeft;
    }

    public override void _Process(double delta)
    {
        if (currentTimeLeft <= 0)
        {
            //restart game.
            GetTree().ReloadCurrentScene();
        }

        if (currentTimeLeft <= secondCounter)
        {
            //play audio
            AudioManager.Instance.Play("ticktock");
            //decrement second counter
            secondCounter--;
        }
        currentTimeLeft -= (float)delta * GameState.Instance.timeScale;


        timeLabel.Text = "DEBUG\nTime Left: " + currentTimeLeft.ToString("0") + "\nTime Scale: " + GameState.Instance.timeScale.ToString();
    }

    void LoadWorld(string levelName, bool useSpawnPos)
    {
        string previousWorldName = "";

        //check if there's a level already loaded, if so, remove it first
        if (currentWorld != null) {
            previousWorldName = currentWorld.Name;
            currentWorld.QueueFree();
            currentWorld = null;
        }

        //load the level
        currentWorld = SceneManager.Instance.GetScene(levelName).Instantiate() as World;
        GetParent().AddChild(currentWorld);

        //set the player's position
        if (useSpawnPos)
        {
            Vector2 spawnPos = Vector2.Zero;

            for (int i = 0; i < currentWorld.GetPortals().Length; i++)
            {
                if (currentWorld.GetPortals()[i].GetWorldName() == previousWorldName)
                {
                    player.GlobalPosition = currentWorld.GetPortals()[i].GetSpawnPos();
                } 
            }
        }

        SetTimeScale(currentWorld.GetTimeScale());

    }

    public void SetTimeScale(float newTimeScale)
    {
        GameState.Instance.timeScale = newTimeScale;
    }
}
