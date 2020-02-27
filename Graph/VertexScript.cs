using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Behaviour script for the vertex game object.
/// </summary>
public class VertexScript : MonoBehaviour
{
    [SerializeField] private int index; // Integer representing the order in which the vertex was created.
    [SerializeField] private ConstrainedVector3 positionConstraints, rotationConstraints; // The constraints applied to the vertex's transform.
    [SerializeField] private List<EdgeScript> inputEdges, outputEdges; // List of vertices entering/exiting the vertex.

    public int Index
    {
        get { return index; }
        set { index = value; }
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

    public List<EdgeScript> InputEdges
    {
        get { return inputEdges; }
        set { inputEdges = value; }
    }

    public List<EdgeScript> OutputEdges
    {
        get { return outputEdges; }
        set { outputEdges = value; }
    }

    public Transform VertexTransform { get; set; } // Cached transform of the vertex.

    public List<int> MarkedEdges { get; set; } // List the number of times each edge has been visited during a traversal.

    public MouseDragScript MouseDragScript { get; set; } // The component that enables the mouse drag.

    private void Awake()
    {
        VertexTransform = transform;
        MouseDragScript = GetComponent<MouseDragScript>();
    }

    private void Start()
    {
        MarkedEdges = VertexTransform.root.GetComponent<EdgesMarker>().Edges;
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
        Translate(MouseDragScript.ObjectDisplacement());
        OutputEdgesIKSolver(1);
        InputEdgesIKSolver(1);
    }

    // Move the game object.
    public void Translate(Vector3 displacement)
    {
        VertexTransform.position += displacement;
    }

    // Solve the inverse kinematic for all the edges exiting the vertex.
    public void OutputEdgesIKSolver(int maxVisits)
    {
        for (int i = 0; i < OutputEdges.Count; ++i)
        {
            if (OutputEdges[i].MarkedEdges[OutputEdges[i].Index] < maxVisits)
            {
                //Debug.Log("Edge #" + OutputEdges[i].Index);
                OutputEdges[i].MarkedEdges[OutputEdges[i].Index] += 1;

                Vector3 from = OutputEdges[i].TerminalPointOffset;
                Vector3 to = VertexTransform.position - OutputEdges[i].TerminalPoint.VertexTransform.position;
                Vector3 axis = Vector3.Cross(from, to);
                float angle = Vector3.SignedAngle(from, to, axis);
                Quaternion rotation = Quaternion.AngleAxis(angle, axis);

                OutputEdges[i].Rotate(rotation);
                OutputEdges[i].EdgeTransform.position = VertexTransform.position + OutputEdges[i].InitialPointOffset;
                OutputEdges[i].TerminalPoint.VertexTransform.position = OutputEdges[i].EdgeTransform.position - OutputEdges[i].TerminalPointOffset;

                OutputEdges[i].TerminalPoint.OutputEdgesIKSolver(maxVisits);
                OutputEdges[i].TerminalPoint.InputEdgesIKSolver(maxVisits);
            }
        }
    }

    // Solve the inverse kinematic for all the edges entering the vertex.
    public void InputEdgesIKSolver(int maxVisits)
    {
        for (int i = 0; i < InputEdges.Count; ++i)
        {
            if (InputEdges[i].MarkedEdges[InputEdges[i].Index] < maxVisits)
            {
                //Debug.Log("Edge #" + InputEdges[i].Index);
                InputEdges[i].MarkedEdges[InputEdges[i].Index] += 1;

                Vector3 from = InputEdges[i].InitialPointOffset;
                Vector3 to = VertexTransform.position - InputEdges[i].InitialPoint.VertexTransform.position;
                Vector3 axis = Vector3.Cross(from, to);
                float angle = Vector3.SignedAngle(from, to, axis);
                Quaternion rotation = Quaternion.AngleAxis(angle, axis);
                
                InputEdges[i].Rotate(rotation);
                InputEdges[i].EdgeTransform.position = VertexTransform.position + InputEdges[i].TerminalPointOffset;
                InputEdges[i].InitialPoint.VertexTransform.position = InputEdges[i].EdgeTransform.position - InputEdges[i].InitialPointOffset;

                InputEdges[i].InitialPoint.InputEdgesIKSolver(maxVisits);
                InputEdges[i].InitialPoint.OutputEdgesIKSolver(maxVisits);
            }
        }
    }
}

/* References:
 * https://www.rosroboticslearning.com/forward-kinematics
 * https://forum.patagames.com/posts/t501-What-Is-Transformation-Matrix-and-How-to-Use-It
 */
