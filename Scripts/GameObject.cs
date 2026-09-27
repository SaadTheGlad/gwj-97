using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class GameObject : Node2D
{
    private List<BaseComponent> components = new List<BaseComponent>();

    public void InitComponents()
    {
        foreach(var node in GetChildren())
        {
            if(node is BaseComponent component)
            {
                components.Add(component);
            }
        }
    }

    public void BindComponents()
    {
        if (components == null) return;

        foreach (BaseComponent component in components)
        {
            component.Bind(this);
        }
    }

    public T GetComponent<T>()
    {
        if (components == null) return default(T);

        foreach (BaseComponent component in components)
        {
            if (component is T confirmedComponent)
            {
                return confirmedComponent;
            }
        }

        return default(T);
    }

    public override void _EnterTree()
    {
        InitComponents();
        BindComponents();
    }
}
