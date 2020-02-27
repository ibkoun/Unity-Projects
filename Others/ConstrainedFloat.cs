using System;
using UnityEngine;

// TODO: Review this class.

/// <summary>
/// Applies restriction to a float value.
/// </summary>
[Serializable]
public struct ConstrainedFloat
{
    [SerializeField] private bool freeze; // If true, value cannot be modified.
    [SerializeField] private bool clamp; // If true, value must be in range of minValue and maxValue.

    [HideInInspector]
    [SerializeField] private float initialValue;
    [SerializeField] private float minValue, maxValue, value;

    public bool Freeze
    {
        get { return freeze; }
        set { freeze = value; }
    }

    public bool Clamp
    {
        get { return clamp; }
        set { clamp = value; }
    }

    public float MinValue
    {
        get { return minValue; }
        set { maxValue = value; }
    }

    public float MaxValue
    {
        get { return maxValue; }
        set { maxValue = value; }
    }

    public float InitialValue
    {
        get { return initialValue; }
        set { initialValue = value; }
    }

    public float Value
    {
        get { return value; }
        set
        {
            if (!freeze)
            {
                if (clamp) this.value = Mathf.Clamp(value, minValue, maxValue);
                else this.value = value;
            }
        }
    }
}
