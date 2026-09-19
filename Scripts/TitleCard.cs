using Godot;
using System;

public partial class TitleCard : PanelContainer
{
    [Export] private AnimationPlayer animationPlayer;
    [Export] private Label titleCardLabel;


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

        //setting up the text for the title card
        titleCardLabel.Text = $"WORLD ENDS IN {GetTextFromTimeInSeconds(WorldManager.Instance.timeLoopTime)}*";
    }

    private string GetTextFromTimeInSeconds(float totalSeconds)
    {
        int minutes = (int)Mathf.Floor(totalSeconds / 60f);
        int seconds = (int)(totalSeconds - minutes * 60f);
        if(seconds == 0)
        {
            return $"{minutes.ToString()} minutes";
        }

        return $"{minutes.ToString()} minutes & {seconds.ToString()} seconds";
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
