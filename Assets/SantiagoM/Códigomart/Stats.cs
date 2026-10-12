using System;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierType { Flat, Percent } // Usar llaves en estilo Allman; Mover el enum público a su propio archivo

public class StatModifier
{
    public readonly float Value;
    public readonly ModifierType Type;
    public readonly object Source;

    public StatModifier(float value, ModifierType type, object source)
    {
        Value = value;
        Type = type;
        Source = source;
    }
}

[Serializable]
public class Stat
{
    public float BaseValue; // Respetar el orden de los miembros

    private readonly List<StatModifier> _modifiers = new List<StatModifier>(); // Respetar el orden de los miembros

    public float Value // Respetar el orden de los miembros
    {
        get
        {
            float flat = 0f;
            float percent = 0f;

            foreach (StatModifier m in _modifiers)
            {
                if (m.Type == ModifierType.Flat) flat += m.Value; // Agregar llaves al if
                else percent += m.Value;
            }

            return Mathf.Max(0f, (BaseValue + flat) * (1f + percent));
        }
    }

    public void AddModifier(StatModifier modifier)
    {
        _modifiers.Add(modifier);
    }

    // Quita todos los modificadores que vinieron de un mismo item
    public void RemoveModifiersFrom(object source)
    {
        _modifiers.RemoveAll(m => m.Source == source);
    }
}