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

    //World resources. I added this to figure out which ones I've loaded and which not
    [Export] private WorldResource[] worldResources;

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
        CallDeferred("LoadWorld", "BlueWorld");


        currentTimeLeft = timeLoopTime;
        secondCounter = (int)timeLoopTime;
    }

    void LoadWorld(string worldName)
    {
        GD.Print("Loading World...");

        currentWorld = GetWorld(worldName);
        GetParent().AddChild(currentWorld);

        //changes the time scale based on the world you're in
        SetTimeScale(currentWorld.GetTimeScale());
    }

    World GetWorld(string worldName)
    {
        foreach(var worldResource in worldResources)
        {
            if(worldResource.worldName == worldName)
            {
                return (World)worldResource.worldPackedScene.Instantiate();
            }
        }

        GD.PushError("World entered does not exist.");
        return null;
    }

    void SetWorldVisited(string worldName)
    {
        foreach (var worldResource in worldResources)
        {
            if (worldResource.worldName == worldName)
            {
                worldResource.hasLoadedWorld = true;
                return;
            }
        }

        GD.PushError("World entered does not exist.");
    }

    bool WasWorldVisited(string worldName)
    {
        foreach (var worldResource in worldResources)
        {
            if (worldResource.worldName == worldName)
            {
                return worldResource.hasLoadedWorld;
            }
        }

        GD.PushError("World entered does not exist.");
        return false;
    }

    public override void _Process(double delta)
    {
        if (currentTimeLeft <= 0)
        {
            RestartGame();
        }

        VisualizeTime(delta);
    }

    public void RemoveObjectFromDictionary(string path)
    {
        persistantObjectsDictionary.Remove(path);
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

    public void SaveObjectsState()
    {
        GD.Print("Saving Data...");

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
                    string key = persistantObject.GetKey();

                    //If the dictionary does not have the key, then we add it. If it does, then we update it.
                    if (!persistantObjectsDictionary.ContainsKey(key))
                    {
                        persistantObjectsDictionary.Add(key, persistantObject.Save());
                    }
                    else
                    {
                        persistantObjectsDictionary[key] = persistantObject.Save();
                    }

                    var savedPersistantObject = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)persistantObjectsDictionary[key]);
                    string worldName = savedPersistantObject["World"].ToString();
                    string nodeName = savedPersistantObject["Name"].ToString();


                    GD.Print($"- {nodeName} in {worldName} with path: {savedPersistantObject["Path"]}");
                }
            }
        }
    }

    public void LoadObjectsState()
    {
        GD.Print("Loading Data...");

        //load the persistant object positions based on the dictionary
        foreach (string persistantObjectKey in persistantObjectsDictionary.Keys)
        {
            var persistantObjectData = 
                new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)persistantObjectsDictionary[persistantObjectKey]);
            foreach (var (key, value) in persistantObjectData)
            {
                string worldName = persistantObjectData["World"].ToString();
                if (worldName != currentWorld.GetWorldName()) continue;

                Vector2 position = (Vector2)persistantObjectData["Position"];
                Node2D node = GetNode(persistantObjectData["Path"].ToString()) as Node2D;
                node.SetPosition(position);
            }
        }

        //loops through all persistant objects and if it doesn't find them in the dictionary then it deletes them
        var persistantObjects = GetTree().GetNodesInGroup("Persistant");

        foreach (Node persistantObject in persistantObjects)
        {
            GD.Print($"Checking: {persistantObject.GetPath()}");

            //If the dictionary does not have the path of the object and if it exists then remove it.
            if (!persistantObjectsDictionary.ContainsKey(persistantObject.GetPath()) && GetNodeOrNull(persistantObject.GetPath()) != null && WasWorldVisited(currentWorld.GetWorldName()))
            {
                persistantObject.QueueFree();
                GD.Print($"Removed {persistantObject.Name}. No longer part of world.");
            }
        }
    }


    private void PrintDictionary()
    {
        GD.Print("Objects currently in dictionary: ");

        foreach (string key in persistantObjectsDictionary.Keys)
        {

            var nodeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)persistantObjectsDictionary[key]);
            string worldName = nodeData["World"].ToString();
            string nodeName = nodeData["Name"].ToString();


            GD.Print($"- {nodeName} in {worldName} with path: {nodeData["Path"]}");
        }
    }


    void ChangeWorld(string worldName, bool useSpawnPos)
    {
        //NOTE: This doesn't run when we load from _Ready() for some reason. I don't know if that'll be a problem...
        string targetWorldNameForPortal = "";

        //save the objects info before unloading the world
        SaveObjectsState();

        //check if there's a level already loaded, if so, remove it first
        if (currentWorld != null) {
            //we set the previous world name to keep track of which portal to go to when we tp
            targetWorldNameForPortal = currentWorld.Name;
            //TODO !!THIS SHOULDN'T BE FREE BUT IT KINDA FIXES IT!!
            currentWorld.Free();
            currentWorld = null;
        }

        //load the level
        LoadWorld(worldName);

        //loads the state of the objects
        LoadObjectsState();

        SetWorldVisited(worldName);

        //set the player's position
        if (useSpawnPos)
        {
            Vector2 spawnPos = Vector2.Zero;

            for (int i = 0; i < currentWorld.GetPortals().Length; i++)
            {
                if (currentWorld.GetPortals()[i].GetTargetWorldName() == targetWorldNameForPortal)
                {
                    player.GlobalPosition = currentWorld.GetPortals()[i].GetSpawnPos();
                    break;
                }
            }
        }
    }
}
