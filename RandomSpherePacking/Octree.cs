using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tree data structure in which each internal node has exactly eigth children.
/// </summary>
public class Octree<T>
{
    public Octant<T> Root { get; set; }

    public Octree(Vector3 center, Vector3 size)
    {
        Root = new Octant<T>();
        Root.Centerpoint = center;
        Root.Depth = 0;
        Root.Size = size; 
    }

    // Get the child of the chosen octant based on it's position.
    public Octant<T> Get(Octant<T> octant, Vector3 point)
    {
        if (octant.Leaves == null) return octant;
        else
        {
            int index;
            Vector3 direction = point - octant.Centerpoint;
            if (direction.x > 0 && direction.y > 0 && direction.z < 0) index = 0;
            else if (direction.x < 0 && direction.y > 0 && direction.z < 0) index = 1;
            else if (direction.x < 0 && direction.y < 0 && direction.z < 0) index = 2;
            else if (direction.x > 0 && direction.y < 0 && direction.z < 0) index = 3;
            else if (direction.x > 0 && direction.y > 0 && direction.z > 0) index = 4;
            else if (direction.x < 0 && direction.y > 0 && direction.z > 0) index = 5;
            else if (direction.x < 0 && direction.y < 0 && direction.z > 0) index = 6;
            else if (direction.x > 0 && direction.y < 0 && direction.z > 0) index = 7;
            else return octant;
            return octant.Leaves[index];
        }
    }

    // TODO: Add search methods
}


