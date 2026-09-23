using Godot;
using System;

public partial class GameState : Node
{
    public static GameState Instance { get; private set; }

    public bool hasMetSlumpedMan;
    public bool playerCanMove = false;
    public bool hasPlayedCutscene = false;

    //conditions for wion
    public bool hasDefusedRobot = false;
    public bool hasSatiatedSlumpedMan = false;
    public bool gameWon = false;
    public int timeRemainingInt;

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
        hasInvokedGameWon = false;
        hasDefusedRobot = false;
        hasSatiatedSlumpedMan = false;
        gameWon = false;
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

    public bool hasInvokedGameWon = false;

    public override void _Process(double delta)
    {
        if(hasDefusedRobot && hasSatiatedSlumpedMan && !hasInvokedGameWon)
        {
            EventManager.GameWon?.Invoke();
            GD.Print("Game won!");

            //do some panel here and pause the game
            hasInvokedGameWon = true;
            GetTree().Paused = true;
        }
    }
}
