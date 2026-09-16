using System;
using Godot;
using System.Numerics;

public static class EventManager
{
    public static Action<string, int, float> ExampleEvent;

    public static Action<string, bool> WorldEntered;

    public static Action GameOver;

    public static Action ResetVelocity;

    public static Action SaveState;
}