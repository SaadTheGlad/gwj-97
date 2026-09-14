using Godot;
using System;

public partial class PlayerCanTakeDamage : CanTakeDamage
{
    public override void Die()
    {
        CallDeferred("RestartGame");
    }

    private void RestartGame()
    {
        EventManager.GameOver?.Invoke();

    }
}
