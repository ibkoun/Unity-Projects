using System;
using UnityEngine;

[Serializable]
public class Sphere
{
    [SerializeField] private Vector3 centerpoint;
    [SerializeField] private float radius;

    public Sphere(Vector3 centerpoint, float radius)
    {
        this.centerpoint = centerpoint;
        this.radius = radius;
    }

    public Vector3 Centerpoint
    {
        get { return centerpoint; }
        set { centerpoint = value; }
    }

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }
}
