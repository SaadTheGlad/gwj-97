using Godot;

[GlobalClass]
public partial class NPC : GameObject
{
    [Export] private InteractionArea interactArea;
    [Export] public string currentWorldName;
}
