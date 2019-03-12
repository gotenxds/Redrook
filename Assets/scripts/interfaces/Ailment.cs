using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ailment {
    private readonly string name;
    public string abbreviation;

    public Ailment(string name, string abbreviation)
    {
        this.name = name;
        this.abbreviation = abbreviation;
    }

    public override bool Equals(object obj)
    {
        return obj is Ailment ailment &&
               name == ailment.name;
    }

    public override int GetHashCode()
    {
        return 363513814 + EqualityComparer<string>.Default.GetHashCode(name);
    }
}