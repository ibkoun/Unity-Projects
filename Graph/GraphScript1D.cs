using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity script for the graph object.
/// </summary>
public class GraphScript1D : MonoBehaviour
{
    [SerializeField] private List<Vertex> vertices; // List of all the vertices.
    [SerializeField] private SerializableBooleanMatrix1D adjacencyMatrix; // List of all the pairs of vertices connected by an edge.
    [SerializeField] private SerializableFloatMatrix1D distanceMatrix; // List of all the distances between each pair of vertices.

    public List<Vertex> Vertices
    {
        get { return vertices; }
        set { vertices = value; }
    }

    public SerializableBooleanMatrix1D AdjacencyMatrix
    {
        get { return adjacencyMatrix; }
        set { adjacencyMatrix = value; }
    }

    public SerializableFloatMatrix1D DistanceMatrix
    {
        get { return distanceMatrix; }
        set { distanceMatrix = value; }
    }

    // Draw a sphere for each vertex, and draw a line for each pair of vertices connected by an edge.
    private void OnDrawGizmosSelected()
    {
        if (vertices == null) vertices = new List<Vertex>();
        if (adjacencyMatrix == null) adjacencyMatrix = new SerializableBooleanMatrix1D();
        if (distanceMatrix == null) distanceMatrix = new SerializableFloatMatrix1D();
        for (int i = 0; i < vertices.Count; ++i)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(vertices[i].Position, 0.05f);
            for (int j = i + 1; j < vertices.Count; ++j)
            {
                // Output edges are colored green (row i to column j).
                if (adjacencyMatrix.Get(i, j) && distanceMatrix.Get(i, j) > 0)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(vertices[i].Position, vertices[j].Position);
                }
                // Input edges are colored red (column j to row i).
                if (adjacencyMatrix.Get(j, i) && distanceMatrix.Get(j, i) > 0)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(vertices[j].Position, vertices[i].Position);
                }
            }
        }
    }

    // Update the adjacency and distance matrices when some changes has been made in the custom inspector.
    private void OnValidate()
    {
        if (vertices == null) vertices = new List<Vertex>();

        if (adjacencyMatrix == null) adjacencyMatrix = new SerializableBooleanMatrix1D();
        adjacencyMatrix.RowsCount = vertices.Count;
        adjacencyMatrix.ColumnsCount = vertices.Count;

        if (distanceMatrix == null) distanceMatrix = new SerializableFloatMatrix1D();
        distanceMatrix.RowsCount = vertices.Count;
        distanceMatrix.ColumnsCount = vertices.Count;

        Vector3 vector;
        float distance;
        for (int i = 0; i < vertices.Count; ++i)
        {
            for (int j = i; j < vertices.Count; ++j)
            {
                if (i == j) distanceMatrix.Set(i, j, 0); // No vertices are connected to themselves.
                else
                {
                    vector = vertices[i].Position - vertices[j].Position;
                    distance = vector.magnitude;
                    distanceMatrix.Set(i, j, distance);
                    distanceMatrix.Set(j, i, distance);
                }
            }
        }
    }

    public void BuildGraph(float vertexRadius, float edgeRadius)
    {
        // Create an empty game object for the graph.
        GameObject graph = new GameObject("Graph");
        graph.AddComponent<VerticesMarker>();
        VerticesMarker verticesMarker = graph.GetComponent<VerticesMarker>();
        verticesMarker.Vertices = new List<int>();
        graph.AddComponent<EdgesMarker>();
        EdgesMarker edgesMarker = graph.GetComponent<EdgesMarker>();
        edgesMarker.Edges = new List<int>();
        Transform graphTransform = graph.transform;

        // Create an empty game object to contain all the vertices.
        VertexScript[] vertexScripts = new VertexScript[vertices.Count];
        GameObject verticesGroup = new GameObject("Vertices");
        Transform verticesTransform = verticesGroup.transform;
        verticesTransform.parent = graphTransform;

        // Create a game object for each vertex.
        for (int i = 0; i < vertices.Count; ++i)
        {
            verticesMarker.Vertices.Add(0);

            // Resize the collider used for all the vertices.
            GameObject vertex = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            vertex.name = "Vertex " + i;
            SphereCollider sphereCollider = vertex.GetComponent<SphereCollider>();
            sphereCollider.radius = vertexRadius;
            var mesh = vertex.GetComponent<MeshFilter>().mesh;
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
            Transform vertexTransform = vertex.transform;
            vertexTransform.parent = verticesTransform;
            vertexTransform.position = vertices[i].Position;

            // Add the vertex behaviour script.
            vertex.AddComponent<VertexScript>();
            VertexScript vertexScript = vertex.GetComponent<VertexScript>();
            vertexScript.PositionConstraints = new ConstrainedVector3(vertexTransform.position);
            vertexScript.RotationConstraints = new ConstrainedVector3(vertexTransform.eulerAngles);
            vertexScript.Index = i;
            vertexScripts[i] = vertexScript;
            if (vertexScript.InputEdges == null) vertexScript.InputEdges = new List<EdgeScript>();
            if (vertexScript.OutputEdges == null) vertexScript.OutputEdges = new List<EdgeScript>();

            // Add a rigidbody (optional).
            vertex.AddComponent<Rigidbody>();
            Rigidbody rigidbody = vertex.GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            // Add the component that enables the mouse drag.
            vertex.AddComponent<MouseDragScript>();
        }

        // Create an empty game object to contain all the edges.
        GameObject edgesGroup = new GameObject("Edges");
        Transform edgesTransform = edgesGroup.transform;
        edgesTransform.parent = graphTransform;

        // Create a game object for each edge.
        for (int i = 0; i < vertices.Count; ++i)
        {
            for (int j = i + 1; j < vertices.Count; ++j)
            {
                if (adjacencyMatrix.Get(i, j) && distanceMatrix.Get(i, j) > 0) // Output edges.
                {
                    edgesMarker.Edges.Add(0);

                    // Resize the collider used for all the edges.
                    GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    edge.name = "Edge " + i + "-" + j;
                    CapsuleCollider capsuleCollider = edge.GetComponent<CapsuleCollider>();
                    capsuleCollider.radius = edgeRadius;
                    capsuleCollider.height = distanceMatrix.Get(i, j);
                    var mesh = edge.GetComponent<MeshFilter>().mesh;
                    var baseMeshVertices = mesh.vertices;
                    var newMeshVertices = new Vector3[baseMeshVertices.Length];
                    for (int x = 0; x < newMeshVertices.Length; ++x)
                    {
                        var newMeshVertex = baseMeshVertices[x];
                        newMeshVertex.x *= capsuleCollider.bounds.size.x / mesh.bounds.size.x;
                        newMeshVertex.y *= capsuleCollider.bounds.size.y / mesh.bounds.size.y;
                        newMeshVertex.z *= capsuleCollider.bounds.size.z / mesh.bounds.size.z;
                        newMeshVertices[x] = newMeshVertex;
                    }
                    mesh.vertices = newMeshVertices;
                    mesh.RecalculateBounds();
                    edge.GetComponent<MeshFilter>().mesh = mesh;
                    Transform edgeTransform = edge.transform;
                    edgeTransform.parent = edgesTransform;
                    edgeTransform.position = Vector3.Lerp(vertices[i].Position, vertices[j].Position, 0.5f);

                    // Add the edge behaviour script.
                    edge.AddComponent<EdgeScript>();
                    EdgeScript edgeScript = edge.GetComponent<EdgeScript>();
                    edgeScript.InitialPoint = vertexScripts[i];
                    edgeScript.TerminalPoint = vertexScripts[j];
                    edgeScript.PositionConstraints = new ConstrainedVector3(edgeTransform.position);
                    edgeScript.RotationConstraints = new ConstrainedVector3(edgeTransform.eulerAngles);
                    edgeScript.Index = edgesMarker.Edges.Count - 1;
                    edgeScript.InitialPoint.OutputEdges.Add(edgeScript);
                    edgeScript.TerminalPoint.InputEdges.Add(edgeScript);

                    // Align the edge along the pair of vertices.
                    Vector3 direction = vertices[j].Position - vertices[i].Position;
                    edgeScript.Length = distanceMatrix.Get(i, j);
                    Vector3 axis = Vector3.Cross(edgeTransform.up, direction);
                    float angle = Vector3.SignedAngle(edgeTransform.up, direction, axis);
                    edgeTransform.rotation = Quaternion.AngleAxis(angle, axis);

                    // Add a rigidbody (optional).
                    edge.AddComponent<Rigidbody>();
                    Rigidbody rigidbody = edge.GetComponent<Rigidbody>();
                    rigidbody.useGravity = false;
                    rigidbody.isKinematic = true;

                    // Add the component that enables the mouse drag.
                    edge.AddComponent<MouseDragScript>();
                }
                else if (adjacencyMatrix.Get(j, i) && distanceMatrix.Get(j, i) > 0) // Input edges.
                {
                    edgesMarker.Edges.Add(0);
                    GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    edge.name = "Edge " + j + "-" + i;
                    CapsuleCollider capsuleCollider = edge.GetComponent<CapsuleCollider>();
                    capsuleCollider.radius = edgeRadius;
                    capsuleCollider.height = distanceMatrix.Get(j, i);
                    var mesh = edge.GetComponent<MeshFilter>().mesh;
                    var baseMeshVertices = mesh.vertices;
                    var newMeshVertices = new Vector3[baseMeshVertices.Length];
                    for (int x = 0; x < newMeshVertices.Length; ++x)
                    {
                        var newMeshVertex = baseMeshVertices[x];
                        newMeshVertex.x *= capsuleCollider.bounds.size.x / mesh.bounds.size.x;
                        newMeshVertex.y *= capsuleCollider.bounds.size.y / mesh.bounds.size.y;
                        newMeshVertex.z *= capsuleCollider.bounds.size.z / mesh.bounds.size.z;
                        newMeshVertices[x] = newMeshVertex;
                    }
                    mesh.vertices = newMeshVertices;
                    mesh.RecalculateBounds();
                    edge.GetComponent<MeshFilter>().mesh = mesh;
                    Transform edgeTransform = edge.transform;
                    edgeTransform.parent = edgesTransform;
                    edgeTransform.position = Vector3.Lerp(vertices[j].Position, vertices[i].Position, 0.5f);

                    edge.AddComponent<EdgeScript>();
                    EdgeScript edgeScript = edge.GetComponent<EdgeScript>();
                    edgeScript.InitialPoint = vertexScripts[j];
                    edgeScript.TerminalPoint = vertexScripts[i];
                    edgeScript.PositionConstraints = new ConstrainedVector3(edgeTransform.position);
                    edgeScript.RotationConstraints = new ConstrainedVector3(edgeTransform.eulerAngles);
                    edgeScript.Index = edgesMarker.Edges.Count - 1;
                    edgeScript.InitialPoint.OutputEdges.Add(edgeScript);
                    edgeScript.TerminalPoint.InputEdges.Add(edgeScript);

                    Vector3 direction = vertices[i].Position - vertices[j].Position;
                    edgeScript.Length = distanceMatrix.Get(j, i);
                    Vector3 axis = Vector3.Cross(edgeTransform.up, direction);
                    float angle = Vector3.SignedAngle(edgeTransform.up, direction, axis);
                    edgeTransform.rotation = Quaternion.AngleAxis(angle, axis);

                    edge.AddComponent<Rigidbody>();
                    Rigidbody rigidbody = edge.GetComponent<Rigidbody>();
                    rigidbody.useGravity = false;
                    rigidbody.isKinematic = true;

                    edge.AddComponent<MouseDragScript>();
                }
            }
        }
    }
}

/* References:
 * https://answers.unity.com/questions/523289/change-size-of-mesh-at-runtime.html
 */
