using Godot;

[GlobalClass]
public partial class GameobjectNPC : GameObject
{
    [Export] private InteractionArea interactArea;
    [Export] public string currentWorldName;
}
