using Godot;
using System;

public interface IComponentable
{
    public void InitComponents();

    public void BindComponents();

    public T GetComponent<T>();

}
