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

        _actor.AddToGroup("Persistant");
    }

    public void SetPosition(Vector2 position)
    {
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
            { "Path", actor.GetPath() },
            { "Name", actor.Name},
            { "Position", actor2D.GlobalPosition},
            { "World", WorldManager.Instance.GetCurrentWorld().Name}
        };
    }

}
