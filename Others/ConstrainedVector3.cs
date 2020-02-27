using System;
using UnityEngine;

// TODO: Review this class.

/// <summary>
/// Wrapper class for Vector3 to allow constraints.
/// </summary>
[Serializable]
public class ConstrainedVector3
{
    [SerializeField] private ConstrainedFloat X, Y, Z;

    public Vector3 Vector
    {
        get { return new Vector3(x, y, z); }
        set 
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }

    public Vector3 MinVector
    {
        get { return new Vector3(MinX, MinY, MinZ); }
        set
        {
            MinX = value.x;
            MinY = value.y;
            MinZ = value.z;
        }
    }

    public Vector3 MaxVector
    {
        get { return new Vector3(MaxX, MaxY, MaxZ); }
        set
        {
            MaxX = value.x;
            MaxY = value.y;
            MaxZ = value.z;
        }
    }

    public ConstrainedVector3(Vector3 vector)
    {
        X = new ConstrainedFloat();
        Y = new ConstrainedFloat();
        Z = new ConstrainedFloat();
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public float InitialX
    {
        get { return X.InitialValue; }
        set { X.InitialValue = value; }
    }

    public float InitialY
    {
        get { return Y.InitialValue; }
        set { Y.InitialValue = value; }
    }

    public float InitialZ
    {
        get { return Z.InitialValue; }
        set { Z.InitialValue = value; }
    }

    public float x
    {
        get { return X.Value; }
        set { X.Value = value; }
    }

    public float y
    {
        get { return Y.Value; }
        set { Y.Value = value; }
    }
    public float z
    {
        get { return Z.Value; }
        set { Z.Value = value; }
    }

    public float MinX
    {
        get { return X.MinValue; }
        set { X.MinValue = value; }
    }

    public float MinY
    {
        get { return Y.MinValue; }
        set { Y.MinValue = value; }
    }

    public float MinZ
    {
        get { return Z.MinValue; }
        set { Z.MinValue = value; }
    }

    public float MaxX
    {
        get { return X.MaxValue; }
        set { X.MaxValue = value; }
    }

    public float MaxY
    {
        get { return Y.MaxValue; }
        set { Y.MaxValue = value; }
    }

    public float MaxZ
    {
        get { return Z.MaxValue; }
        set { Z.MaxValue = value; }
    }
}
