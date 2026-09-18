using Godot;
using System;

public partial class GameState : Node
{
    public static GameState Instance { get; private set; }

    public bool hasMetSlumpedMan;
    public bool playerCanMove = false;
    public bool hasPlayedCutscene = false;

    public void EnablePlayerMove()
    {
        playerCanMove = true;
        GetTree().Paused = false;
    }

    public void DisablePlayerMove() => playerCanMove = false;

    public void RestartGame()
    {
        GetTree().ReloadCurrentScene();
        GetTree().Paused = true;
    }

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

    }

    public override void _Ready()
    {
        GetTree().Paused = true;
    }
}
