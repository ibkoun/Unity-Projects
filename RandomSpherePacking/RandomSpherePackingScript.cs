using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Script to insert spheres randomly inside a box.
/// </summary>
public class RandomSpherePackingScript : MonoBehaviour
{
    [SerializeField] private Vector3 centerpoint, size;
    [SerializeField] private int numberOfSpheres;
    [SerializeField] private bool randomRadius;
    [SerializeField] private float radius; // Radius for every sphere.
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;
    [SerializeField] private bool randomDistance;
    [SerializeField] private float distance; // Minimum distance between each sphere.
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private int maxIterations;
    [SerializeField] private List<Sphere> spheres; // List of all the spheres that don't overlap.

    private Octree<Sphere> octree; // Data structure used for collision detection.
    private readonly System.Random seed = new System.Random();

    // Check whether a sphere overlaps the inside of a bounding box.
    public bool SphereWithinBox(float sphereRadius, Vector3 sphereCenterpoint, Vector3 boxSize, Vector3 boxCenterpoint)
    {
        Vector3 halfBox = boxSize / 2;
        bool fitLeft = sphereCenterpoint.x - sphereRadius > boxCenterpoint.x - halfBox.x;
        bool fitRight = sphereCenterpoint.x + sphereRadius < boxCenterpoint.x + halfBox.x;
        bool fitBottom = sphereCenterpoint.y - sphereRadius > boxCenterpoint.y - halfBox.y;
        bool fitTop = sphereCenterpoint.y + sphereRadius < boxCenterpoint.y + halfBox.y;
        bool fitBack = sphereCenterpoint.z - sphereRadius > boxCenterpoint.z - halfBox.z;
        bool fitFront = sphereCenterpoint.z + sphereRadius < boxCenterpoint.z + halfBox.z;
        return fitLeft && fitRight && fitBottom && fitTop && fitBack && fitFront;
    }

    // Check whether a sphere intersects another one.
    public bool SphereIntersectsSphere(float sphereRadius1, float sphereRadius2, Vector3 sphereCenterpoint1, Vector3 sphereCenterpoint2, float distance)
    {
        return (sphereCenterpoint1 - sphereCenterpoint2).sqrMagnitude < Mathf.Pow(sphereRadius1 + sphereRadius2 + distance, 2);
    }

    // Generate a random coordinate for a sphere.
    public Vector3 RandomSphereCenterpoint(float radius, Vector3 boundary, Vector3 centerpoint, System.Random seed)
    {
        float x = (float)seed.NextDouble() * (boundary.x - 2 * radius) + centerpoint.x - boundary.x / 2 + radius;
        float y = (float)seed.NextDouble() * (boundary.y - 2 * radius) + centerpoint.y - boundary.y / 2 + radius;
        float z = (float)seed.NextDouble() * (boundary.z - 2 * radius) + centerpoint.z - boundary.z / 2 + radius;
        return new Vector3(x, y, z);
    }

    // Return a list of all the children octants overlapped by a sphere.
    public List<Octant<Sphere>> OverlappedOctants(Octant<Sphere> octant, Sphere sphere, float distance)
    {
        List<Octant<Sphere>> octants = new List<Octant<Sphere>>();
        if (octant.Leaves == null) return octants;
        Vector3 direction = sphere.Centerpoint - octant.Centerpoint;
        bool XY = Mathf.Approximately(direction.z, 0); // Centerpoint intersects the XY-plane.
        bool XZ = Mathf.Approximately(direction.y, 0); // Centerpoint intersects the XZ-plane.
        bool YZ = Mathf.Approximately(direction.x, 0); // Centerpoint intersects the YZ-plane.

        // Check if the surface of the sphere overlaps the YZ-plane (east and west).
        bool overlapsLeft = sphere.Centerpoint.x - sphere.Radius - distance < octant.Centerpoint.x;
        bool overlapsRight = sphere.Centerpoint.x + sphere.Radius + distance > octant.Centerpoint.x;

        // Check if the surface of the sphere overlaps the XZ-plane (up and down).
        bool overlapsBottom = sphere.Centerpoint.y - sphere.Radius - distance < octant.Centerpoint.y;
        bool overlapsTop = sphere.Centerpoint.y + sphere.Radius + distance > octant.Centerpoint.y;

        // Check if the surface of the sphere overlaps the XY-plane (north and south).
        bool overlapsBack = sphere.Centerpoint.z - sphere.Radius - distance < octant.Centerpoint.z;
        bool overlapsFront = sphere.Centerpoint.z + sphere.Radius + distance > octant.Centerpoint.z;

        // Centerpoint intersects the XY-plane only.
        if (XY && !XZ && !YZ)
        {
            if (direction.x > 0 && direction.y > 0) // Centerpoint overlaps octant 1 and 5.
            {
                octants.Add(octant.Leaves[0]);
                octants.Add(octant.Leaves[4]);
                if (overlapsLeft) // Surface overlaps octant 2 and 6.
                {
                    octants.Add(octant.Leaves[1]);
                    octants.Add(octant.Leaves[5]);
                }
                if (overlapsBottom) // Surface overlaps octant 4 and 8.
                {
                    octants.Add(octant.Leaves[3]);
                    octants.Add(octant.Leaves[7]);
                }
                if (overlapsLeft 
                    && overlapsBottom) // Surface overlaps octant 3 and 7.
                {
                    octants.Add(octant.Leaves[2]);
                    octants.Add(octant.Leaves[6]);
                }
            }
            else if (direction.x < 0 && direction.y > 0) // Centerpoint overlaps octant 2 and 6.
            {
                octants.Add(octant.Leaves[1]);
                octants.Add(octant.Leaves[5]);
                if (overlapsRight) // Surface overlaps octant 1 and 5.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[4]);
                }
                if (overlapsBottom) // Surface overlaps octant 3 and 7.
                {
                    octants.Add(octant.Leaves[2]);
                    octants.Add(octant.Leaves[6]);
                }
                if (overlapsRight && overlapsBottom) // Surface overlaps octant 4 and 8.
                {
                    octants.Add(octant.Leaves[3]);
                    octants.Add(octant.Leaves[7]);
                }
            }
            else if (direction.x < 0 && direction.y < 0) // Centerpoint overlaps octant 3 and 7.
            {
                octants.Add(octant.Leaves[2]);
                octants.Add(octant.Leaves[6]);
                if (overlapsRight) // Surface overlaps octant 4 and 8.
                {
                    octants.Add(octant.Leaves[3]);
                    octants.Add(octant.Leaves[7]);
                }
                if (overlapsTop) // Surface overlaps octant 2 and 6.
                {
                    octants.Add(octant.Leaves[1]);
                    octants.Add(octant.Leaves[5]);
                }
                if (overlapsRight && overlapsTop) // Surface overlaps octant 1 and 5.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[4]);
                }
            }
            else if (direction.x > 0 && direction.y < 0) // Centerpoint overlaps octant 4 and 8.
            {
                octants.Add(octant.Leaves[3]);
                octants.Add(octant.Leaves[7]);
                if (overlapsLeft) // Surface overlaps octant 3 and 7.
                {
                    octants.Add(octant.Leaves[2]);
                    octants.Add(octant.Leaves[6]);
                }
                if (overlapsTop) // Surface overlaps octant 1 and 4.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[3]);
                }
                if (overlapsLeft && overlapsTop) // Surface overlaps octant 2 and 6.
                {
                    octants.Add(octant.Leaves[1]);
                    octants.Add(octant.Leaves[5]);
                }
            }
        }
        // Centerpoint intersects the XZ-plane only.
        else if (!XY && XZ && !YZ)
        {
            if (direction.x > 0 && direction.z < 0) // Centerpoint overlaps octant 1 and 4.
            {
                octants.Add(octant.Leaves[0]);
                octants.Add(octant.Leaves[3]);
                if (overlapsLeft) // Surface overlaps octant 2 and 3.
                {
                    octants.Add(octant.Leaves[1]);
                    octants.Add(octant.Leaves[2]);
                }
                if (overlapsFront) // Surface overlaps octant 5 and 8.
                {
                    octants.Add(octant.Leaves[4]);
                    octants.Add(octant.Leaves[7]);
                }
                if (overlapsLeft && overlapsFront) // Surface overlaps octant 6 and 7.
                {
                    octants.Add(octant.Leaves[5]);
                    octants.Add(octant.Leaves[6]);
                }
            }
            else if (direction.x < 0 && direction.z < 0) // Centerpoint overlaps octant 2 and 3.
            {
                octants.Add(octant.Leaves[1]);
                octants.Add(octant.Leaves[2]);
                if (overlapsRight) // Surface overlaps octant 1 and 4.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[3]);
                }
                if (overlapsFront) // Surface overlaps octant 6 and 7.
                {
                    octants.Add(octant.Leaves[5]);
                    octants.Add(octant.Leaves[6]);
                }
                if (overlapsRight && overlapsFront) // Surface overlaps octant 5 and 8.
                {
                    octants.Add(octant.Leaves[4]);
                    octants.Add(octant.Leaves[7]);
                }
            }
            else if (direction.x > 0 && direction.z > 0) // Centerpoint overlaps octant 5 and 8.
            {
                octants.Add(octant.Leaves[4]);
                octants.Add(octant.Leaves[7]);
                if (overlapsLeft) // Surface overlaps octant 6 and 7.
                {
                    octants.Add(octant.Leaves[5]);
                    octants.Add(octant.Leaves[6]);
                }
                if (overlapsBack) // Surface overlaps octant 1 and 4.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[3]);
                }
                if (overlapsLeft && overlapsBack) // Surface overlaps octant 2 and 3.
                {
                    octants.Add(octant.Leaves[1]);
                    octants.Add(octant.Leaves[2]);
                }
            }
            else if (direction.x < 0 && direction.z > 0) // Centerpoint overlaps octant 6 and 7.
            {
                octants.Add(octant.Leaves[5]);
                octants.Add(octant.Leaves[6]);
                if (overlapsRight) // Surface overlaps octant 5 and 8.
                {
                    octants.Add(octant.Leaves[4]);
                    octants.Add(octant.Leaves[7]);
                }
                if (overlapsBack) // Surface overlaps octant 2 and 3.
                {
                    octants.Add(octant.Leaves[1]);
                    octants.Add(octant.Leaves[2]);
                }
                if (overlapsRight && overlapsBack) // Surface overlaps octant 1 and 4.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[3]);
                }
            }
        }
        // Centerpoint intersects the YZ-plane only.
        else if (!XY && !XZ && YZ) 
        {
            if (direction.y > 0 && direction.z < 0) // Centerpoint overlaps octant 1 and 2.
            {
                octants.Add(octant.Leaves[0]);
                octants.Add(octant.Leaves[1]);
                if (overlapsBottom) // Surface overlaps octant 3 and 4.
                {
                    octants.Add(octant.Leaves[2]);
                    octants.Add(octant.Leaves[3]);
                }
                if (overlapsFront) // Surface overlaps octant 5 and 6.
                {
                    octants.Add(octant.Leaves[4]);
                    octants.Add(octant.Leaves[5]);
                }
                if (overlapsBottom && overlapsFront) // Surface overlaps octant 7 and 8.
                {
                    octants.Add(octant.Leaves[6]);
                    octants.Add(octant.Leaves[7]);
                }
            }
            else if (direction.y < 0 && direction.z < 0) // Centerpoint overlaps octant 3 and 4.
            {
                octants.Add(octant.Leaves[2]);
                octants.Add(octant.Leaves[3]);
                if (overlapsTop) // Surface overlaps octant 1 and 2.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[1]);
                }
                if (overlapsFront) // Surface overlaps octant 7 and 8.
                {
                    octants.Add(octant.Leaves[6]);
                    octants.Add(octant.Leaves[7]);
                }
                if (overlapsTop && overlapsFront) // Surface overlaps octant 5 and 6.
                {
                    octants.Add(octant.Leaves[4]);
                    octants.Add(octant.Leaves[5]);
                }
            }
            else if (direction.y > 0 && direction.z > 0) // Centerpoint overlaps octant 5 and 6.
            {
                octants.Add(octant.Leaves[4]);
                octants.Add(octant.Leaves[5]);
                if (overlapsBottom) // Surface overlaps octant 7 and 8.
                {
                    octants.Add(octant.Leaves[6]);
                    octants.Add(octant.Leaves[7]);
                }
                if (overlapsBack) // Surface overlaps octant 1 and 2.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[1]);
                }
                if (overlapsBottom && overlapsBack) // Surface overlaps octant 3 and 4.
                {
                    octants.Add(octant.Leaves[2]);
                    octants.Add(octant.Leaves[3]);
                }
            }
            else if (direction.y < 0 && direction.z > 0) // Centerpoint overlaps octant 7 and 8.
            {
                octants.Add(octant.Leaves[6]);
                octants.Add(octant.Leaves[7]);
                if (overlapsTop) // Surface overlaps octant 5 and 6.
                {
                    octants.Add(octant.Leaves[4]);
                    octants.Add(octant.Leaves[5]);
                }
                if (overlapsBack) // Surface overlaps octant 3 and 4.
                {
                    octants.Add(octant.Leaves[2]);
                    octants.Add(octant.Leaves[3]);
                }
                if (overlapsTop && overlapsBack) // Surface overlaps octant 1 and 2.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[1]);
                }
            }
        }
        // Centerpoint intersects the XY-plane and XZ-plane.
        else if (XY && XZ && !YZ)
        {
            if (direction.x > 0) // Centerpoint overlaps octant 1, 4, 5 and 8.
            {
                octants.Add(octant.Leaves[0]);
                octants.Add(octant.Leaves[3]);
                octants.Add(octant.Leaves[4]);
                octants.Add(octant.Leaves[7]);
                if (overlapsLeft) // Surface overlaps octant 2, 3, 6 and 7.
                {
                    octants.Add(octant.Leaves[1]);
                    octants.Add(octant.Leaves[2]);
                    octants.Add(octant.Leaves[5]);
                    octants.Add(octant.Leaves[6]);
                }
            }
            else if (direction.x < 0) // Centerpoint overlaps octant 2, 3, 6 and 7.
            {
                octants.Add(octant.Leaves[1]);
                octants.Add(octant.Leaves[2]);
                octants.Add(octant.Leaves[5]);
                octants.Add(octant.Leaves[6]);
                if (overlapsRight) // Surface overlaps octant 1, 4, 5 and 8.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[3]);
                    octants.Add(octant.Leaves[4]);
                    octants.Add(octant.Leaves[7]);
                }
            }
        }
        // Centerpoint intersects the XY-plane and YZ-plane.
        else if (XY && !XZ && YZ)
        {
            if (direction.y > 0) // Centerpoint overlaps octant 1, 2, 5 and 6.
            {
                octants.Add(octant.Leaves[0]);
                octants.Add(octant.Leaves[1]);
                octants.Add(octant.Leaves[4]);
                octants.Add(octant.Leaves[5]);
                if (overlapsBottom) // Surface overlaps octant 3, 4, 7 and 8.
                {
                    octants.Add(octant.Leaves[2]);
                    octants.Add(octant.Leaves[3]);
                    octants.Add(octant.Leaves[6]);
                    octants.Add(octant.Leaves[7]);
                }
            }
            else if (direction.y < 0) // Centerpoint overlaps octant 3, 4, 7 and 8.
            {
                octants.Add(octant.Leaves[2]);
                octants.Add(octant.Leaves[3]);
                octants.Add(octant.Leaves[6]);
                octants.Add(octant.Leaves[7]);
                if (overlapsTop) // Surface overlaps octant 1, 2, 5 and 6.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[1]);
                    octants.Add(octant.Leaves[4]);
                    octants.Add(octant.Leaves[5]);
                }
            }
        }
        // Centerpoint intersects the XZ-plane and YZ-plane.
        else if (!XY && XZ && YZ)
        {
            if (direction.z > 0) // Centerpoint overlaps octant 5, 6, 7 and 8.
            {
                octants.Add(octant.Leaves[4]);
                octants.Add(octant.Leaves[5]);
                octants.Add(octant.Leaves[6]);
                octants.Add(octant.Leaves[7]);
                if (overlapsBack) // Surface overlaps octant 1, 2, 3 and 4.
                {
                    octants.Add(octant.Leaves[0]);
                    octants.Add(octant.Leaves[1]);
                    octants.Add(octant.Leaves[2]);
                    octants.Add(octant.Leaves[3]);
                }
            }
            else if (direction.z < 0) // Centerpoint overlaps octant 1, 2, 3 and 4.
            {
                octants.Add(octant.Leaves[0]);
                octants.Add(octant.Leaves[1]);
                octants.Add(octant.Leaves[2]);
                octants.Add(octant.Leaves[3]);
                if (overlapsFront) // Surface overlaps octant 5, 6, 7 and 8.
                {
                    octants.Add(octant.Leaves[4]);
                    octants.Add(octant.Leaves[5]);
                    octants.Add(octant.Leaves[6]);
                    octants.Add(octant.Leaves[7]);
                }
            }

        }
        // Centerpoint is at the center of the octant.
        else if (XY && XZ && YZ) 
        {
            for (int j = 0; j < octant.Leaves.Length; j++)
            {
                octants.Add(octant.Leaves[j]);
            }
        }
        // Centerpoint is within the boundary of the octant.
        else
        {
            Octant<Sphere> child = octant.Get(sphere.Centerpoint);
            octants.Add(child);
            switch (child.Index)
            {
                case 0:
                    if (overlapsLeft) octants.Add(octant.Leaves[1]); // Surface overlaps octant 2.
                    if (overlapsBottom) octants.Add(octant.Leaves[3]); // Surface overlaps octant 4.
                    if (overlapsFront) octants.Add(octant.Leaves[4]); // Surface overlaps octant 5.
                    if (overlapsLeft && overlapsBottom) octants.Add(octant.Leaves[2]); // Surface overlaps octant 3.
                    if (overlapsLeft && overlapsFront) octants.Add(octant.Leaves[5]); // Surface overlaps octant 6.
                    if (overlapsBottom && overlapsFront) octants.Add(octant.Leaves[7]); // Surface overlaps octant 8.
                    if (overlapsLeft  && overlapsBottom && overlapsFront) octants.Add(octant.Leaves[6]); // Surface overlaps octant 7.
                    break;
                case 1:
                    if (overlapsRight) octants.Add(octant.Leaves[0]); // Surface overlaps octant 1.
                    if (overlapsBottom) octants.Add(octant.Leaves[2]); // Surface overlaps octant 3.
                    if (overlapsFront) octants.Add(octant.Leaves[5]); // Surface overlaps octant 6.
                    if (overlapsRight && overlapsBottom) octants.Add(octant.Leaves[3]); // Surface overlaps octant 4.
                    if (overlapsRight && overlapsFront) octants.Add(octant.Leaves[4]); // Surface overlaps octant 5.
                    if (overlapsBottom && overlapsFront) octants.Add(octant.Leaves[6]); // Surface overlaps octant 7.
                    if (overlapsRight && overlapsBottom && overlapsFront) octants.Add(octant.Leaves[7]); // Surface overlaps octant 8.
                    break;
                case 2:
                    if (overlapsRight) octants.Add(octant.Leaves[3]); // Surface overlaps octant 4.
                    if (overlapsTop) octants.Add(octant.Leaves[1]); // Surface overlaps octant 2.
                    if (overlapsFront) octants.Add(octant.Leaves[6]); // Surface overlaps octant 7.
                    if (overlapsRight && overlapsTop) octants.Add(octant.Leaves[0]); // Surface overlaps octant 1.
                    if (overlapsRight && overlapsFront) octants.Add(octant.Leaves[7]); // Surface overlaps octant 8.
                    if (overlapsTop && overlapsFront) octants.Add(octant.Leaves[5]); // Surface overlaps octant 6.
                    if (overlapsRight && overlapsTop && overlapsFront) octants.Add(octant.Leaves[4]); // Surface overlaps octant 5.
                    break;
                case 3:
                    if (overlapsLeft) octants.Add(octant.Leaves[2]); // Surface overlaps octant 3.
                    if (overlapsTop) octants.Add(octant.Leaves[0]); // Surface overlaps octant 1.
                    if (overlapsFront) octants.Add(octant.Leaves[7]); // Surface overlaps octant 8.
                    if (overlapsLeft && overlapsTop) octants.Add(octant.Leaves[1]); // Surface overlaps octant 2.
                    if (overlapsLeft && overlapsFront) octants.Add(octant.Leaves[6]); // Surface overlaps octant 7.
                    if (overlapsTop && overlapsFront) octants.Add(octant.Leaves[4]); // Surface overlaps octant 5.
                    if (overlapsLeft && overlapsTop && overlapsFront) octants.Add(octant.Leaves[5]); // Surface overlaps octant 6.
                    break;
                case 4:
                    if (overlapsLeft) octants.Add(octant.Leaves[5]); // Surface overlaps octant 6.
                    if (overlapsBottom) octants.Add(octant.Leaves[7]); // Surface overlaps octant 8.
                    if (overlapsBack) octants.Add(octant.Leaves[0]); // Surface overlaps octant 1.
                    if (overlapsLeft && overlapsBottom) octants.Add(octant.Leaves[6]); // Surface overlaps octant 7.
                    if (overlapsLeft && overlapsBack) octants.Add(octant.Leaves[1]); // Surface overlaps octant 2.
                    if (overlapsBottom && overlapsBack) octants.Add(octant.Leaves[3]); // Surface overlaps octant 4.
                    if (overlapsLeft && overlapsBottom && overlapsBack) octants.Add(octant.Leaves[2]); // Surface overlaps octant 3.
                    break;
                case 5:
                    if (overlapsRight) octants.Add(octant.Leaves[4]); // Surface overlaps octant 5.
                    if (overlapsBottom) octants.Add(octant.Leaves[6]); // Surface overlaps octant 7.
                    if (overlapsBack) octants.Add(octant.Leaves[1]); // Surface overlaps octant 2.
                    if (overlapsRight && overlapsBottom) octants.Add(octant.Leaves[7]); // Surface overlaps octant 8.
                    if (overlapsRight && overlapsBack) octants.Add(octant.Leaves[0]); // Surface overlaps octant 1.
                    if (overlapsBottom && overlapsBack) octants.Add(octant.Leaves[2]); // Surface overlaps octant 3.
                    if (overlapsRight && overlapsBottom && overlapsBack) octants.Add(octant.Leaves[3]); // Surface overlaps octant 4.
                    break;
                case 6:
                    if (overlapsRight) octants.Add(octant.Leaves[7]); // Surface overlaps octant 8.
                    if (overlapsTop) octants.Add(octant.Leaves[5]); // Surface overlaps octant 6.
                    if (overlapsBack) octants.Add(octant.Leaves[2]); // Surface overlaps octant 3.
                    if (overlapsRight && overlapsTop) octants.Add(octant.Leaves[4]); // Surface overlaps octant 5.
                    if (overlapsRight && overlapsBack) octants.Add(octant.Leaves[3]); // Surface overlaps octant 4.
                    if (overlapsTop && overlapsBack) octants.Add(octant.Leaves[1]); // Surface overlaps octant 2.
                    if (overlapsRight && overlapsTop && overlapsBack) octants.Add(octant.Leaves[0]); // Surface overlaps octant 1.
                    break;
                case 7:
                    if (overlapsLeft) octants.Add(octant.Leaves[6]); // Surface overlaps octant 7.
                    if (overlapsTop) octants.Add(octant.Leaves[4]); // Surface overlaps octant 5.
                    if (overlapsBack) octants.Add(octant.Leaves[3]); // Surface overlaps octant 4.
                    if (overlapsLeft && overlapsTop) octants.Add(octant.Leaves[5]); // Surface overlaps octant 6.
                    if (overlapsLeft && overlapsBack) octants.Add(octant.Leaves[2]); // Surface overlaps octant 3.
                    if (overlapsTop && overlapsBack) octants.Add(octant.Leaves[0]); // Surface overlaps octant 1.
                    if (overlapsLeft && overlapsTop && overlapsBack) octants.Add(octant.Leaves[1]); // Surface overlaps octant 2.
                    break;
            }
        }
        return octants;
    }

    /*
     * Naive method that iterates through each pair of spheres to test whether they collide with each other
     * (used to confirm that the following collision detection methods work).
     */
    public bool SpheresCollisionDetection()
    {
        for (int i = 0; i < spheres.Count; i++)
        {
            for (int j = i + 1; j < spheres.Count; j++)
            {
                if (SphereIntersectsSphere(spheres[i].Radius, spheres[j].Radius, spheres[i].Centerpoint, spheres[j].Centerpoint, distance))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Test whether a sphere collides with another one by iterating through all the existing spheres.
    public bool NaiveCollisionDetection(List<Sphere> spheres, Sphere sphere, float distance)
    {
        int comparisons = 0;
        for (int i = 0; i < spheres.Count; i++)
        {
            comparisons++;
            if (SphereIntersectsSphere(sphere.Radius, spheres[i].Radius, sphere.Centerpoint, spheres[i].Centerpoint, distance))
            {
                Debug.Log("Comparisons until collision (naive): " + comparisons);
                return false;
            }
        }
        Debug.Log("Comparisons without collision (naive): " + comparisons);
        return true;
    }

    // Test whether a sphere collides with another one by iterating randomly through existing spheres.

    public bool RandomCollisionDetection(List<Sphere> spheres, Sphere sphere, float distance, System.Random seed)
    {
        Sphere[] array = new Sphere[spheres.Count];
        spheres.CopyTo(array);
        List<Sphere> list = array.ToList();
        int comparisons = 0;
        while (list.Count > 0)
        {
            comparisons++;
            int i = seed.Next(0, list.Count);
            if (SphereIntersectsSphere(sphere.Radius, list[i].Radius, sphere.Centerpoint, list[i].Centerpoint, distance))
            {
                Debug.Log("Comparisons until collision (random): " + comparisons);
                return true;
            }
            list.RemoveAt(i);
        }
        Debug.Log("Comparisons without collision (random): " + comparisons);
        return false;
    }

    // Test whether a sphere collides with another one by iterating through all the octants the sphere overlaps.
    public bool OctreeCollisionDetection(Octant<Sphere> octant, Sphere sphere, float distance)
    {
        int comparisons = 0;
        List<Octant<Sphere>> queue = new List<Octant<Sphere>>(); // Octants that can contain spheres that will collide with the new one.
        List<Octant<Sphere>> octants = new List<Octant<Sphere>>(); // Octants that will be updated if they passed the collision test.

        // Inserting the first sphere.
        if (octant.Contents.Count == 0)
        {
            if (SphereWithinBox(sphere.Radius, sphere.Centerpoint, octant.Size, octant.Centerpoint))
            {
                octant.Contents.Add(sphere);
                return false;
            }
            return true;
        }

        // Inserting the spheres following the first one.
        queue.Add(octant);
        while (queue.Count > 0)
        {
            Octant<Sphere> candidate = queue[0]; // Octant that will be tested for a collision.
            queue.RemoveAt(0);

            // The octant doesn't contain any child.
            if (candidate.Leaves == null)
            {
                // The octant contains at least one sphere.
                if (candidate.Contents.Count > 0)
                {
                    for (int i = 0; i < candidate.Contents.Count; i++)
                    {
                        comparisons++;
                        Sphere content = candidate.Contents[i];
                        
                        // This is where we will test for any collision.
                        if (SphereIntersectsSphere(sphere.Radius, content.Radius, sphere.Centerpoint, content.Centerpoint, distance))
                        {
                            Debug.Log("Comparisons until collision (octree): " + comparisons);
                            return true;
                        }
                    }

                    // Check whether the new sphere can fit inside a child of a partitioned octant.
                    bool partition = true;
                    if (!SphereWithinBox(sphere.Radius, sphere.Centerpoint, candidate.Size / 2, sphere.Centerpoint))
                    {
                        // Check whether an existing sphere can fit inside a child of a partitioned octant.
                        for (int i = 0; i < candidate.Contents.Count; i++)
                        {
                            if (!SphereWithinBox(candidate.Contents[i].Radius, candidate.Contents[i].Centerpoint, candidate.Size / 2, candidate.Contents[i].Centerpoint))
                            {
                                partition = false;
                                break;
                            }
                        }
                    }
                    // Only if the new sphere and the existing ones can fit inside a child of a partitioned octant.
                    if (partition)
                    {
                        candidate.Partition();
                        for (int i = 0; i < candidate.Contents.Count; i++)
                        {
                            // Propagate the contents of an octant to its children.
                            List<Octant<Sphere>> list = OverlappedOctants(candidate, candidate.Contents[i], distance);
                            for (int j = 0; j < list.Count; j++)
                            {
                                candidate.Leaves[list[j].Index].Contents.Add(candidate.Contents[i]);
                            }
                        }
                    }
                    List<Octant<Sphere>> candidates = OverlappedOctants(candidate, sphere, distance);
                    octants = octants.Concat(candidates).ToList(); // Cache the octants that will be updated.
                    queue = queue.Concat(candidates).ToList(); // Add the octants that will have to pass the collision test.
                }
            }
            else
            {
                for (int i = 0; i < candidate.Contents.Count; i++)
                {
                    comparisons++;
                    Sphere content = candidate.Contents[i];
                    if (SphereIntersectsSphere(sphere.Radius, content.Radius, sphere.Centerpoint, content.Centerpoint, distance))
                    {
                        Debug.Log("Comparisons until collision (octree): " + comparisons);
                        return true;
                    }
                }
                List<Octant<Sphere>> candidates = OverlappedOctants(candidate, sphere, distance);
                octants = octants.Concat(candidates).ToList();
                queue = queue.Concat(candidates).ToList();
            }
        }

        // Update the octree if no collision has been detected.
        queue.Add(octant); // Start with the root.
        queue = queue.Concat(octants).ToList(); // Add the octants overlapped by the new sphere.
        while (queue.Count > 0)
        {
            Octant<Sphere> candidate = queue[0];
            if (candidate.Leaves == null) candidate.Contents.Add(sphere); // Only update the leaves of the octree.
            queue.RemoveAt(0);
        }
        Debug.Log("Comparisons without collision (octree): " + comparisons);
        return false;
    }

    // Create a number of random spheres inside a bounding box.
    public void CreateRandomSpheres()
    {
        octree = new Octree<Sphere>(centerpoint, size);
        spheres.Clear();
        distance = randomDistance ? (float)seed.NextDouble() * (maxDistance - minDistance) + minRadius : distance;

        // For each sphere, find an unoccupied position inside the box without overlapping the existing ones.
        for (int i = 0; i < numberOfSpheres; i++)
        {
            float sphereRadius = randomRadius ? (float)seed.NextDouble() * (maxRadius - minRadius) + minRadius : radius;
            Sphere sphere = new Sphere(RandomSphereCenterpoint(sphereRadius, octree.Root.Size, octree.Root.Centerpoint, seed), sphereRadius);
            int iterations = 0;
            while (iterations < maxIterations)
            {
                NaiveCollisionDetection(spheres, sphere, distance);
                RandomCollisionDetection(spheres, sphere, distance, seed);
                if (!OctreeCollisionDetection(octree.Root, sphere, distance))
                {
                    spheres.Add(sphere);
                    break;
                }
                else
                {
                    sphere.Centerpoint = RandomSphereCenterpoint(sphereRadius, octree.Root.Size, octree.Root.Centerpoint, seed);
                    iterations++;
                }
            }
        }

        // Create a game object for each sphere.
        GameObject spheresParent = GameObject.Find("Spheres");
        if (spheresParent)
        {
            foreach (Transform sphere in spheresParent.transform)
            {
                DestroyImmediate(sphere.gameObject);
            }
            DestroyImmediate(spheresParent);
        }
        if (spheres.Count > 0)
        {
            spheresParent = new GameObject("Spheres");
            spheresParent.transform.position = centerpoint;
            for (int i = 0; i < spheres.Count; i++)
            {
                GameObject sphereObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphereObject.name = "Sphere " + i;
                SphereCollider sphereCollider = sphereObject.GetComponent<SphereCollider>();
                sphereCollider.radius = spheres[i].Radius;
                var mesh = randomRadius ? sphereObject.GetComponent<MeshFilter>().mesh : sphereObject.GetComponent<MeshFilter>().sharedMesh;
                var baseMeshVertices = mesh.vertices;
                var newMeshVertices = new Vector3[baseMeshVertices.Length];
                for (int x = 0; x < newMeshVertices.Length; ++x)
                {
                    var newMeshVertex = baseMeshVertices[x];
                    newMeshVertex.x *= sphereCollider.bounds.size.x / mesh.bounds.size.x;
                    newMeshVertex.y *= sphereCollider.bounds.size.y / mesh.bounds.size.y;
                    newMeshVertex.z *= sphereCollider.bounds.size.z / mesh.bounds.size.z;
                    newMeshVertices[x] = newMeshVertex;
                }
                mesh.vertices = newMeshVertices;
                mesh.RecalculateBounds();
                sphereObject.transform.position = spheres[i].Centerpoint;
                sphereObject.transform.parent = spheresParent.transform;
            }

        }
        Debug.Log("Collision: " + SpheresCollisionDetection());
        Debug.Log("Spheres: " + spheres.Count + "/" + numberOfSpheres + " (" + (spheres.Count / (float)numberOfSpheres) * 100f + "%)");
    }

    private void OnDrawGizmosSelected() 
    {
        // Get the coordinate for each corner of the bounding box.
        Vector3 p1 = centerpoint + new Vector3(size.x / 2, size.y / 2, -size.z / 2);
        Vector3 p2 = centerpoint + new Vector3(-size.x / 2, size.y / 2, -size.z / 2);
        Vector3 p3 = centerpoint + new Vector3(-size.x / 2, -size.y / 2, -size.z / 2);
        Vector3 p4 = centerpoint + new Vector3(size.x / 2, -size.y / 2, -size.z / 2);
        Vector3 p5 = centerpoint + new Vector3(size.x / 2, size.y / 2, size.z / 2);
        Vector3 p6 = centerpoint + new Vector3(-size.x / 2, size.y / 2, size.z / 2);
        Vector3 p7 = centerpoint + new Vector3(-size.x / 2, -size.y / 2, size.z / 2);
        Vector3 p8 = centerpoint + new Vector3(size.x / 2, -size.y / 2, size.z / 2);

        // Draw the box (looking in the direction of the positive z-axis)
        Gizmos.color = Color.cyan;

        // Back square (negative z-axis).
        Gizmos.DrawLine(p1, p2); // Top right to top left.
        Gizmos.DrawLine(p2, p3); // Top left to bottom left.
        Gizmos.DrawLine(p3, p4); // Bottom left to bottom right.
        Gizmos.DrawLine(p4, p1); // Bottom right to top left.

        // Front square (positive z-axis).
        Gizmos.DrawLine(p5, p6); // Top right to top left.
        Gizmos.DrawLine(p6, p7); // Top left to bottom left.
        Gizmos.DrawLine(p7, p8); // Bottom left to bottom right.
        Gizmos.DrawLine(p8, p5); // Bottom right to top left.

        // Left square (negative x-axis).
        Gizmos.DrawLine(p3, p7); // Bottom left of the back square to bottom left of the front square.
        Gizmos.DrawLine(p4, p8); // Bottom right of the back square to bottom left on the front square.

        // Right square (positive x-axis).
        Gizmos.DrawLine(p1, p5); // Top right of the back square to top right on the front square.
        Gizmos.DrawLine(p2, p6); // Top left of the back square to top left on the front square.
    }
}

/* References:
 * https://forum.unity.com/threads/centre-of-sphere-for-random-insideunitsphere.83824/
 * https://karthikkaranth.me/blog/generating-random-points-in-a-sphere/
 * https://stackoverflow.com/questions/52440855/how-do-i-create-random-non-overlapping-coordinates
 * http://jdobr.es/blog/algorithmic-art-circle-pack/
 * https://medium.com/@antoine_savine/sobol-sequence-explained-188f422b246b
 * https://blog.kitware.com/octree-collision-imstk/
 */
