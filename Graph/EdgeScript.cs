using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Behaviour script for the edge game object.
/// </summary>
public class EdgeScript : MonoBehaviour
{
    [SerializeField] private int index; // Integer representing the order in which the edge was created.
    [SerializeField] private float length; // Fixed distance between the initial point and the terminal point.
    [SerializeField] private ConstrainedVector3 positionConstraints, rotationConstraints; // The constraints applied to the edge's transform.
    [SerializeField] private VertexScript initialPoint, terminalPoint; // The edge starts from the initial point and ends at the terminal point.

    public int Index
    {
        get { return index; }
        set { index = value; }
    }

    public float Length
    {
        get { return length; }
        set { length = value; }
    }

    public ConstrainedVector3 PositionConstraints
    {
        get { return positionConstraints; }
        set { positionConstraints = value; }
    }

    public ConstrainedVector3 RotationConstraints
    {
        get { return rotationConstraints; }
        set { rotationConstraints = value; }
    }

    public VertexScript InitialPoint
    {
        get { return initialPoint; }
        set { initialPoint = value; }
    }

    public VertexScript TerminalPoint
    {
        get { return terminalPoint; }
        set { terminalPoint = value; }
    }

    public bool Selected { get; set; } // Define whether a mouse click event has been performed on the edge.

    public Vector3 InitialPointOffset { get; set; } // Vector starting from the initial point and ending at the center point of the edge.

    public Vector3 TerminalPointOffset { get; set; } // Vector starting from the terminal point and ending at the center point of the edge.

    public Transform EdgeTransform { get; set; } // Cached transform of the edge.

    public List<int> MarkedEdges { get; set; } // List the number of times each edge has been visited during a traversal.

    public MouseDragScript MouseDragScript { get; set; } // The component that enables the mouse drag.

    

    private void Awake()
    {
        EdgeTransform = transform;
        Selected = false;
        MouseDragScript = GetComponent<MouseDragScript>();
    }

    private void Start()
    {
        InitialPointOffset = EdgeTransform.position - initialPoint.VertexTransform.position;
        TerminalPointOffset = EdgeTransform.position - terminalPoint.VertexTransform.position;
        MarkedEdges = EdgeTransform.root.GetComponent<EdgesMarker>().Edges;
    }

    private void LateUpdate()
    {
        if (MouseDragScript.MouseLeftClick && MouseDragScript.ObjectSelected)
        {
            // TODO: Find a solution for problems with constraints.
            DragAround();
            for (int i = 0; i < MarkedEdges.Count; i++) MarkedEdges[i] = 0; // Reset the number of visits counter.
        }
    }

    // Drag the game object while holding the mouse left click.
    public void DragAround()
    {
        Rotate(MouseDragScript.CameraRotation());
        Translate(MouseDragScript.ObjectDisplacement());
        MarkedEdges[index] += 1; // This edge has been visited once.

        // Solve the inverse kinematic for both endpoints.
        initialPoint.InputEdgesIKSolver(1);
        initialPoint.OutputEdgesIKSolver(1);
        terminalPoint.OutputEdgesIKSolver(1);
        terminalPoint.InputEdgesIKSolver(1);
    }

    // Rotate the game object about it's center point.
    public void Rotate(Quaternion rotation)
    {
        InitialPointOffset = rotation * InitialPointOffset;
        InitialPointOffset = ((length / 2) / InitialPointOffset.magnitude) * InitialPointOffset;
        TerminalPointOffset = rotation * TerminalPointOffset;
        TerminalPointOffset = ((length / 2) / TerminalPointOffset.magnitude) * TerminalPointOffset;
        MouseDragScript.MouseOffset = rotation * MouseDragScript.MouseOffset;
        EdgeTransform.rotation = rotation * EdgeTransform.rotation;
    }

    // Move the game object.
    public void Translate(Vector3 displacement)
    {
        EdgeTransform.position += displacement;
        initialPoint.VertexTransform.position = EdgeTransform.position - InitialPointOffset;
        terminalPoint.VertexTransform.position = EdgeTransform.position - TerminalPointOffset;
    }
}

/* References:
 * https://answers.unity.com/questions/1209461/problem-using-quaternionangleaxis-around-transform.html
 * https://www.youtube.com/watch?v=hbgDqyy8bIw
 * https://www.youtube.com/watch?v=RTc6i-7N3ms
 * https://www.youtube.com/watch?v=10st01Z0jxc
 * https://www.youtube.com/watch?v=UNoX65PRehA
 */
