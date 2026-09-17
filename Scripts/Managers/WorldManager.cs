using Godot;
using Godot.Collections;
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

    //saving variables
    //here the string is the path and the variant is the dictionary
    Dictionary<string, Variant> persistantObjectsDictionary = new Dictionary<string, Variant>();

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
        EventManager.GameOver += RestartGame;
    }

    public override void _ExitTree()
    {
        EventManager.WorldEntered -= LoadWorld;
        EventManager.GameOver -= RestartGame;
    }

    public override void _Ready()
    {
        //loads parent world
        CallDeferred("LoadWorld", "BlueWorld", true);

        currentTimeLeft = timeLoopTime;
        secondCounter = (int)timeLoopTime;
    }

    public override void _Process(double delta)
    {
        if (currentTimeLeft <= 0)
        {
            RestartGame();
        }

        VisualizeTime(delta);
    }

    public World GetCurrentWorld()
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
        currentTimeLeft -= (float)delta * GameState.Instance.timeScale;
        timeLabel.Text = "DEBUG\nTime Left: " + currentTimeLeft.ToString("0") + "\nTime Scale: " + GameState.Instance.timeScale.ToString();
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

    public void SetTimeScale(float newTimeScale)
    {
        GameState.Instance.timeScale = newTimeScale;
    }

    private void PrintDictionary()
    {
        GD.Print("Objects currently in dictionary: ");

        foreach (string key in persistantObjectsDictionary.Keys)
        {

            var nodeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)persistantObjectsDictionary[key]);
            string worldName = nodeData["World"].ToString();
            string nodeName = nodeData["Name"].ToString();


            GD.Print($"- {nodeName} | {worldName} | Path: {nodeData["Path"]}");
        }
    }

    public void SaveObjectsInfo()
    {
        //gets the persistant objects
        var persistantObjects = GetTree().GetNodesInGroup("Persistant");

        //loops through all of them to save their info to the dictionary in the world manager
        foreach (Node node in persistantObjects)
        {
            if (node is IComponentable componentable)
            {
                var persistantObject = componentable.GetComponent<PersistComponent>();
                if (persistantObject != null)
                {
                    string key = persistantObject.Save()["Path"].ToString();

                    //If the dictionary does not have the key, then we add it. If it does, then we update it.
                    if (!persistantObjectsDictionary.ContainsKey(key))
                    {
                        persistantObjectsDictionary.Add(key, persistantObject.Save());
                    }
                    else
                    {
                        persistantObjectsDictionary[key] = persistantObject.Save();
                    }
                }
            }
        }
    }

    public void SaveState()
    {

    }

    public void LoadState()
    {

    }

    void LoadWorld(string levelName, bool useSpawnPos)
    {
        //NOTE: This doesn't run when we load from _Ready() for some reason. I don't know if that'll be a problem...

        string previousWorldName = "";

        //save the objects info before unloading the world
        SaveObjectsInfo();

        //check if there's a level already loaded, if so, remove it first
        if (currentWorld != null) {
            previousWorldName = currentWorld.Name;
            currentWorld.QueueFree();
            currentWorld = null;
        }

        //load the level
        currentWorld = SceneManager.Instance.GetScene(levelName).Instantiate() as World;
        GetParent().AddChild(currentWorld);

        //load the persistant object positions based on the dictionary
        foreach(string objectKey in persistantObjectsDictionary.Keys)
        {
            var nodeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)persistantObjectsDictionary[objectKey]);
            foreach (var (key, value) in nodeData)
            {
                Vector2 position = (Vector2)nodeData["Position"];
                Node2D node = GetNodeOrNull(nodeData["Path"].ToString()) as Node2D;
                if (node != null)
                    node.SetPosition(position);
            }
        }


        //set the player's position
        if (useSpawnPos)
        {
            Vector2 spawnPos = Vector2.Zero;

            for (int i = 0; i < currentWorld.GetPortals().Length; i++)
            {
                if (currentWorld.GetPortals()[i].GetTargetWorldName() == previousWorldName)
                {
                    player.GlobalPosition = currentWorld.GetPortals()[i].GetSpawnPos();
                }
            }
        }

        //changes the time scale based on the world you're in
        SetTimeScale(currentWorld.GetTimeScale());
    }
}
