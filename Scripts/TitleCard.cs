using Godot;
using System;

public partial class TitleCard : PanelContainer
{
    [Export] private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        if (GameState.Instance.hasPlayedCutscene)
        {
            PlayNormalFade();
        }
        else
        {
            PlayCutscene();
        }
    }

    public void PlayCutscene()
    {
        animationPlayer.Play("cutscene");
        GameState.Instance.hasPlayedCutscene = true;
    }

    public void PlayNormalFade()
    {
        animationPlayer.Play("normal_fade");
    }

    //this is just a helper function for the animation player to call it to enable the player to move
    public void EnablePlayerMove()
    {
        GameState.Instance.EnablePlayerMove();
    }

}
