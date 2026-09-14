using Godot;
using System;

public partial class TitleCard : PanelContainer
{
    //this is just a helper function for the animation player to call it to enable the player to move
    public void EnablePlayerMove() => GameState.Instance.EnablePlayerMove();

}
