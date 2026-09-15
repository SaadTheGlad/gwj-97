using Godot;

[GlobalClass]
public partial class PersistComponent : BaseComponent
{
    private Node2D actor2D;

    public override void Bind(Node _actor)
    {
        base.Bind(_actor);

        if(_actor is Node2D _actor2D)
        {
            actor2D = _actor2D;
        }
    }

    public void SetPosition(Vector2 position)
    {
        GD.Print($"Saved position: {position}");
        actor2D.GlobalPosition = position;
    }

    public Vector2 GetPosition()
    {
        return actor2D.GlobalPosition;
    }

    public Godot.Collections.Dictionary<string, Variant> Save()
    {
        return new Godot.Collections.Dictionary<string, Variant>()
        {
            { "Path", actor2D.GetPath() },
            { "Position", actor2D.GlobalPosition},
        };
    }
}
