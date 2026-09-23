using DialogueManagerRuntime;
using Godot;
using System;

[GlobalClass]
public partial class TestNPC : StaticBody2D, IComponentable
{
    [Export] private InteractionArea interactArea;

    #region Component Related Code
    [Export] private Node componentHolder;
    private BaseComponent[] components;

    //! Put InitComponents and BindComponents() in _Ready() or _EnterTree() !

    public void InitComponents()
    {
        int componentCount = componentHolder.GetChildCount();

        if (componentCount > 0)
            components = new BaseComponent[componentCount];

        for (int i = 0; i < componentCount; ++i)
        {
            components[i] = componentHolder.GetChild(i) as BaseComponent;
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
    #endregion

    public override void _EnterTree()
    {
        InitComponents();
        BindComponents();
    }

}
