    using Godot;
using System;

public partial class PlayerHealthComponent : HealthComponent
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
