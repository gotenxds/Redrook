using System.Collections.Generic;
using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private Dictionary<Element, float> resistances;

    public float health
    {
        get => hp;
        private set => hp = value;
    }

    public void Damage(AttackMove attackHit)
    {
        health -= CalculateDamageFrom(attackHit);
    }

    protected virtual float CalculateDamageFrom(AttackMove attackHit) {
        var totalDamage = 0f;

        foreach(AttackMove.ElementStruct es in attackHit.elementStructs)
        {
            var resistanceDevisor = (resistances.ContainsKey(es.element) ? resistances[es.element] : 1f);
            totalDamage = es.damage * resistanceDevisor;
        }

        return totalDamage;
    }
}
