using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class Element
{
    [SerializeField] public string name;
    [SerializeField] public Ailment[] ailments;

    public Element(string name, Ailment[] ailments)
    {
        this.name = name;
        this.ailments = ailments;
    }

    public override bool Equals(object obj)
    {
        var element = obj as Element;
        return element != null &&
               name == element.name;
    }

    public override int GetHashCode()
    {
        return 363513814 + EqualityComparer<string>.Default.GetHashCode(name);
    }
}