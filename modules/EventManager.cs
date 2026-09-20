using System;
using Godot;
using System.Numerics;

public static class EventManager
{
    public static Action<string, int, float> ExampleEvent;

    public static Action<string, WorldPortal> WorldEntered;
    public static Action ChangedWorld;

    public static Action GameOver;

    public static Action ResetVelocity;

    public static Action GameWon;
}