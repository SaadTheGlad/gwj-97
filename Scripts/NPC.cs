using Godot;

[GlobalClass]
public partial class NPC : GameObject
{
    [Export] protected InteractionArea interactArea;
    [Export] public string currentWorldName;

    public void SettingCurrentWorldName(string _currentWorldName)
    {
        currentWorldName = _currentWorldName;
    }

    public string GetCurrentWorldName() => currentWorldName;
}
