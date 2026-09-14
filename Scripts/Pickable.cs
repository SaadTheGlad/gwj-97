using Godot;
using System;

public interface IPickable
{
    void PickUp(Node2D theNodeThatPickedYouUp);
}
