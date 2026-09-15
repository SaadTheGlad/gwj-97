using Godot;
using System;

public interface IComponentable
{
    public void InitComponents();

    public T GetComponent<T>();

}
