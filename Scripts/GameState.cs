using Godot;
using System;

public partial class GameState : Node
{
    public static GameState Instance { get; private set; }

    public bool hasMetSlumpedMan;

    public bool playerCanMove;

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
