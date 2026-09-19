using Godot;

public partial class WorldManager : Node
{
    //Singleton
    public static WorldManager Instance { get; private set; }

    //World related variables
    [Export] public World[] worlds;
    World currentWorld;
    [Export] public Node2D player;

    //Time related variables
    [Export] public float timeLoopTime;
    [Export] public Label timeLabel;
    public float currentTimeLeft;


    public float localTime = 0f;

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

        PlayClockSound(delta);
        VisualizeTime(delta);

        if (Input.IsActionJustPressed("restart")) RestartGame();
    }

    public World GetCurrentWorldForPlayer()
    {
        return currentWorld;
    }

    public void PlayClockSound(double delta)
    {
        localTime += (float)delta * currentWorld.GetRelativeTimeFactor();

        if (localTime >= 1f)
        {
            AudioManager.Instance.Play("ticktock");
            localTime = 0f;
        }
    }

    public void VisualizeTime(double delta)
    {
        currentTimeLeft -= (float)delta;
        timeLabel.Text = "DEBUG\nTime Left Relative to you: " + currentTimeLeft.ToString("0") + "\nRelative Time Scale: " + currentWorld.GetRelativeTimeFactor().ToString()
            + "\nControls:\nPickup: Space\nTalk: Z\nRestart: R"
            ;
    }

    public float GetCurrentTime()
    {
        return timeLoopTime - currentTimeLeft;
    }

    private void RestartGame()
    {
        //I just set it to null here cuz if I don't it breaks
        Instance = null;
        GameState.Instance.RestartGame();
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
