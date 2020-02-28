using UnityEngine;

/// <summary>
/// Unit of the octree data structure.
/// </summary>
public class Octant<T>
{
    public Vector3 Center { get; set; } // Center point of the cube representing the octant.

    public Vector3 Size { get; set; } // Width, height and depth of the cube.

    public int Index { get; set; }

    public int Depth { get; set; } // The depth in which resides the octant inside the octree.

    public OctantContent<T> Content { get; set; }

    public Octant<T>[] Leaves { get; set; } // The octants obtained after a partition.

    public Octant<T> Get(int i)
    {
        if (Leaves == null) Leaves = new Octant<T>[8];
        return Leaves[i];
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
                Leaves[i].Size.Scale(new Vector3(0.5f, 0.5f, 0.5f));
                switch(i)
                {
                    case 0:
                        Leaves[i].Center += new Vector3(Center.x / 2, Center.y / 2, -Center.z / 2);
                        break;
                    case 1:
                        Leaves[i].Center += new Vector3(-Center.x / 2, Center.y / 2, -Center.z / 2);
                        break;
                    case 2:
                        Leaves[i].Center -= new Vector3(Center.x / 2, Center.y / 2, Center.z / 2);
                        break;
                    case 3:
                        Leaves[i].Center += new Vector3(Center.x / 2, -Center.y / 2, -Center.z / 2);
                        break;
                    case 4:
                        Leaves[i].Center += new Vector3(Center.x / 2, Center.y / 2, Center.z / 2);
                        break;
                    case 5:
                        Leaves[i].Center += new Vector3(-Center.x / 2, Center.y / 2, Center.z / 2);
                        break;
                    case 6:
                        Leaves[i].Center += new Vector3(-Center.x / 2, -Center.y / 2, -Center.z / 2);
                        break;
                    case 7:
                        Leaves[i].Center += new Vector3(Center.x / 2, -Center.y / 2, Center.z / 2);
                        break;
                }
            }
        }
    }
}
