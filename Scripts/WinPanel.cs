using Godot;
using System;

public partial class WinPanel : PanelContainer
{
    [Export] private Button button;
    [Export] private AnimationPlayer animPlayer;

    public override void _EnterTree()
    {
        EventManager.GameWon += PullUpPanel;
    }

    public override void _ExitTree()
    {
        EventManager.GameWon -= PullUpPanel;
    }

    public void ButtonPressed()
    {
        WorldManager.Instance.RestartGame();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("action3")) EventManager.GameWon?.Invoke();
    }

    public void PullUpPanel()
    {
        animPlayer.Play("pop_up");
    }
}
