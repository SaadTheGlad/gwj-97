using Godot;

[GlobalClass]
public partial class PersistComponent : BaseComponent
{
    [Export(PropertyHint.File, ".tscn")]
    private string packedScenePath { get; set; }

    private Node2D actor2D;
    private bool ignore = false;

    public void SetIgnoreFlag(bool flag) => ignore = flag;
    public bool GetIgnoreFlag() => ignore;

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

    public string GetKey()
    {
        return actor.GetPath();
    }

    public void RemoveFromDictionary()
    {
        WorldManager.Instance.RemoveObjectFromDictionary(actor.GetPath());
    }

    public Godot.Collections.Dictionary<string, Variant> Save()
    {

        return new Godot.Collections.Dictionary<string, Variant>()
        {
            { "Path", actor.GetPath() },
            { "Name", actor.Name},
            { "Position", actor2D.GlobalPosition},
            { "World", WorldManager.Instance.GetCurrentWorld().GetWorldName()},
            { "PackedScene", packedScenePath}
         
        };
    }

}
