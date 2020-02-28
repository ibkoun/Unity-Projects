using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to fit random circles inside a box.
/// </summary>
public class RandomSpheresScript : MonoBehaviour
{
    [SerializeField] private Vector3 center, size;
    [SerializeField] private int numberOfSpheres;
    [SerializeField] private bool randomRadius;
    [SerializeField] private float radius;
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;
    [SerializeField] private int maxIterations;
    [SerializeField] private List<Sphere> spheres;

    private Octree<Vector3> octree;
    private readonly System.Random seed = new System.Random();

    // Create a number of random spheres inside a bounding box.
    public void CreateRandomSpheres()
    {
        octree = new Octree<Vector3>(center, size);
        spheres = new List<Sphere>();
        Octant<Vector3> octant;
        float x, y, z;
        Sphere sphere;
        int k;
        // For each sphere, find an unoccupied position inside the box.
        for (int i = 0; i < numberOfSpheres; i++)
        {
            radius = randomRadius ? (float)seed.NextDouble() * (maxRadius - minRadius) + minRadius : radius;
            k = 0;
            x = (float)seed.NextDouble() * (octree.Root.Size.x - 2 * radius) + octree.Root.Center.x - octree.Root.Size.x / 2 + radius;
            y = (float)seed.NextDouble() * (octree.Root.Size.y - 2 * radius) + octree.Root.Center.y - octree.Root.Size.y / 2 + radius;
            z = (float)seed.NextDouble() * (octree.Root.Size.z - 2 * radius) + octree.Root.Center.z - octree.Root.Size.z / 2 + radius;
            sphere = new Sphere(new Vector3(x, y, z), radius);
            octant = octree.Root;
            while (k < maxIterations)
            {
                octant = octree.Get(octant, sphere.Center);
                // The octant is vacant.
                if (octant.Content == null)
                {
                    octant.Content = new OctantContent<Vector3>(sphere.Center);
                    spheres.Add(sphere);
                    break;
                }
                // The octant contains a sphere.
                else if ((sphere.Center - octant.Content.Value).magnitude - 2 * radius >= 0)
                {
                    // Split the octant into eight smaller octants.
                    if (octant.Leaves == null) octant.Partition();
                    // Verify that the octant's children are all vacant.
                    else
                    {
                        for (int j = 0; j < octant.Leaves.Length; j++)
                        {
                            // Find another octant if at least one child contains a sphere.
                            if (octant.Leaves[j].Content != null &&
                                !((sphere.Center - octant.Leaves[j].Content.Value).magnitude - 2 * radius >= 0))
                            {
                                k++;
                                x = (float)seed.NextDouble() * (octree.Root.Size.x - 2 * radius) + octree.Root.Center.x - octree.Root.Size.x / 2 + radius;
                                y = (float)seed.NextDouble() * (octree.Root.Size.y - 2 * radius) + octree.Root.Center.y - octree.Root.Size.y / 2 + radius;
                                z = (float)seed.NextDouble() * (octree.Root.Size.z - 2 * radius) + octree.Root.Center.z - octree.Root.Size.z / 2 + radius;
                                sphere = new Sphere(new Vector3(x, y, z), radius);
                                octant = octree.Root;
                                break;
                            }
                        }
                    }
                }
                // Find another octant that is vacant.
                else
                {
                    k++;
                    x = (float)seed.NextDouble() * (octree.Root.Size.x - 2 * radius) + octree.Root.Center.x - octree.Root.Size.x / 2 + radius;
                    y = (float)seed.NextDouble() * (octree.Root.Size.y - 2 * radius) + octree.Root.Center.y - octree.Root.Size.y / 2 + radius;
                    z = (float)seed.NextDouble() * (octree.Root.Size.z - 2 * radius) + octree.Root.Center.z - octree.Root.Size.z / 2 + radius;
                    sphere = new Sphere(new Vector3(x, y, z), radius);
                    octant = octree.Root;
                }
            }
        }
    }

    private void OnDrawGizmos() 
    {
        if (spheres != null)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < spheres.Count; i++)
            {
                Gizmos.DrawSphere(spheres[i].Center, spheres[i].Radius);
            }
        }
    }
}

/* References:
 * https://forum.unity.com/threads/centre-of-sphere-for-random-insideunitsphere.83824/
 * https://karthikkaranth.me/blog/generating-random-points-in-a-sphere/
 * https://stackoverflow.com/questions/52440855/how-do-i-create-random-non-overlapping-coordinates
 * https://www.patreon.com/posts/tutorial-to-8929048
 * https://towardsdatascience.com/robotic-control-with-graph-networks-f1b8d22b8c86
 * http://jdobr.es/blog/algorithmic-art-circle-pack/
 * https://medium.com/@antoine_savine/sobol-sequence-explained-188f422b246b
 */
