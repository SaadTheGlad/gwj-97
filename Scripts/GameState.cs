using Godot;
using System;

public partial class GameState : Node
{
    public static GameState Instance { get; private set; }

    public bool hasMetSlumpedMan;
    public bool playerCanMove = true;
    public float robotTimeRemaining = -1f;


    public void EnablePlayerMove() => playerCanMove = true;
    public void DisablePlayerMove() => playerCanMove = false;

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
}
