using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit of the octree data structure.
/// </summary>
public class Octant<T>
{
    public Vector3 Centerpoint { get; set; } // Center point of the cube representing the octant.

    public Vector3 Size { get; set; } // Width, height and depth of the cube.

    public int Index { get; set; }

    public int Depth { get; set; } // The depth in which resides the octant inside the octree.

    public List<T> Contents { get; set; }

    public Octant<T>[] Leaves { get; set; } // The octants obtained after a partition.

    public Octant()
    {
        Contents = new List<T>();
    }

    public Octant<T> Get(int i)
    {
        if (Leaves == null) Leaves = new Octant<T>[8];
        return Leaves[i];
    }

    public Octant<T> Get(Vector3 point)
    {
        if (Leaves == null) return this;
        else
        {
            int index;
            Vector3 direction = point - Centerpoint;
            if (direction.x > 0 && direction.y > 0 && direction.z < 0) index = 0;
            else if (direction.x < 0 && direction.y > 0 && direction.z < 0) index = 1;
            else if (direction.x < 0 && direction.y < 0 && direction.z < 0) index = 2;
            else if (direction.x > 0 && direction.y < 0 && direction.z < 0) index = 3;
            else if (direction.x > 0 && direction.y > 0 && direction.z > 0) index = 4;
            else if (direction.x < 0 && direction.y > 0 && direction.z > 0) index = 5;
            else if (direction.x < 0 && direction.y < 0 && direction.z > 0) index = 6;
            else if (direction.x > 0 && direction.y < 0 && direction.z > 0) index = 7;
            else return this;
            return Leaves[index];
        }
    }

    public void Set(int i, Octant<T> node)
    {
        if (Leaves == null) Leaves = new Octant<T>[8];
        Leaves[i] = node;
    }

    // The function used to divide the cube into 8 smaller cubes.
    public void Partition()
    {
        if (Leaves == null)
        {
            Leaves = new Octant<T>[8];
            for (int i = 0; i < 8; i++)
            {
                Leaves[i] = new Octant<T>();
                Leaves[i].Index = i;
                Leaves[i].Depth = Depth + 1;
                Leaves[i].Size = Vector3.Scale(Size, new Vector3(0.5f, 0.5f, 0.5f));
                Leaves[i].Contents = new List<T>();
                //Debug.Log("Parent: " + Size);
                //Debug.Log("Leaf #" + i + ": " + Leaves[i].Size);
                switch(i)
                {
                    case 0:
                        Leaves[i].Centerpoint += new Vector3(Centerpoint.x / 2, Centerpoint.y / 2, -Centerpoint.z / 2);
                        break;
                    case 1:
                        Leaves[i].Centerpoint += new Vector3(-Centerpoint.x / 2, Centerpoint.y / 2, -Centerpoint.z / 2);
                        break;
                    case 2:
                        Leaves[i].Centerpoint -= new Vector3(Centerpoint.x / 2, Centerpoint.y / 2, Centerpoint.z / 2);
                        break;
                    case 3:
                        Leaves[i].Centerpoint += new Vector3(Centerpoint.x / 2, -Centerpoint.y / 2, -Centerpoint.z / 2);
                        break;
                    case 4:
                        Leaves[i].Centerpoint += new Vector3(Centerpoint.x / 2, Centerpoint.y / 2, Centerpoint.z / 2);
                        break;
                    case 5:
                        Leaves[i].Centerpoint += new Vector3(-Centerpoint.x / 2, Centerpoint.y / 2, Centerpoint.z / 2);
                        break;
                    case 6:
                        Leaves[i].Centerpoint += new Vector3(-Centerpoint.x / 2, -Centerpoint.y / 2, -Centerpoint.z / 2);
                        break;
                    case 7:
                        Leaves[i].Centerpoint += new Vector3(Centerpoint.x / 2, -Centerpoint.y / 2, Centerpoint.z / 2);
                        break;
                }
            }
        }
    }
}
