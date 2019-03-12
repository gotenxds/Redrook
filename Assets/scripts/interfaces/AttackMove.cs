using UnityEngine;
using System;

[Serializable]
public abstract class AttackMove
{
    public struct ElementStruct
    {
        public readonly Element element;
        public readonly float damage;
    }

    public readonly ElementStruct[] elementStructs;
    public readonly string name;
    public bool AOE;
    public float range;
    public Animation animation;
    public int castTime;
    public bool interruptible;

    protected AttackMove(string name, in ElementStruct[] elementStructs)
    {
        this.elementStructs = elementStructs;
        this.name = name;
    }
}
