using Godot;
using System;

[GlobalClass]
public partial class PortalableComponent : BaseComponent
{
    public bool canTeleport = true;
    [Export] private float timeOutTime = 1f;

    async public void SetTimeOut()
    {
        canTeleport = false;
        await ToSignal(GetTree().CreateTimer(timeOutTime), SceneTreeTimer.SignalName.Timeout);
        canTeleport = true;
    }
}
