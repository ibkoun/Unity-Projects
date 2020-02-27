using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Custom editor for the graph script (using one-dimensional adjacency and distance matrices).
/// </summary>
[CustomEditor(typeof(GraphScript1D)), CanEditMultipleObjects]
public class GraphEditor1D : Editor
{
    private SerializedProperty vertices, adjacencyMatrix, distanceMatrix;
    private ReorderableList reorderableVertices;

    private void OnEnable()
    {
        if (vertices == null) vertices = serializedObject.FindProperty("vertices");
        adjacencyMatrix = serializedObject.FindProperty("adjacencyMatrix");
        distanceMatrix = serializedObject.FindProperty("distanceMatrix");
        reorderableVertices = new ReorderableList(serializedObject: serializedObject, elements: vertices, 
            draggable: true, displayHeader: true, displayAddButton: true, displayRemoveButton: true);
        reorderableVertices.drawHeaderCallback = DrawHeaderCallback;
        reorderableVertices.drawElementCallback = DrawElementCallback;
        reorderableVertices.onAddCallback += OnAddCallback;
        reorderableVertices.onRemoveCallback += OnRemoveCallback;
        reorderableVertices.elementHeightCallback += ElementHeightCallback;
        //reorderableVertices.onReorderCallbackWithDetails += OnReorderCallbackWithDetails;
        //reorderableVertices.onChangedCallback += OnChangedCallback;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        reorderableVertices.DoLayoutList();
        DrawMatrix(adjacencyMatrix.FindPropertyRelative("matrix"), "Adjacency matrix", 20, 20);
        DrawMatrix(distanceMatrix.FindPropertyRelative("matrix"), "Distance matrix", 30, 20);
        if (GUILayout.Button("Build graph"))
        {
            ((GraphScript1D)target).BuildGraph(0.05f, 0.025f);
        }
        serializedObject.ApplyModifiedProperties();
    }

    // Customize the appearance of a matrix inside the custom inspector.
    private void DrawMatrix(SerializedProperty matrix, string name, int cellWidth, int cellHeight)
    {
        matrix.isExpanded = EditorGUILayout.Foldout(matrix.isExpanded, name);
        if (matrix.isExpanded)
        {
            // Draw the index for each column.
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < reorderableVertices.count; ++j)
            {
                if (j == 0) EditorGUILayout.LabelField(string.Empty, GUILayout.Width(cellWidth), GUILayout.Height(cellHeight));
                EditorGUILayout.LabelField(j.ToString(), GUILayout.Width(cellWidth), GUILayout.Height(cellHeight));
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < reorderableVertices.count; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < reorderableVertices.count; ++j)
                {
                    // Draw the index for each row.
                    if (j == 0) EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(cellWidth), GUILayout.Height(cellHeight));
                    // Draw the element.
                    EditorGUILayout.PropertyField(matrix.GetArrayElementAtIndex(i * reorderableVertices.count + j),
                        GUIContent.none, GUILayout.Width(cellWidth), GUILayout.Height(cellHeight));
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    // Insert a row and a column (cross) in a matrix based on the index of an element in the reorderable list.
    private void InsertMatrixElement(SerializedProperty matrix, int index)
    {
        for (int i = 0; i < reorderableVertices.count; ++i)
        {
            if (i == index)
            {
                for (int j = 0; j < reorderableVertices.count; ++j)
                {
                    matrix.InsertArrayElementAtIndex(i * reorderableVertices.count + j);
                }
            }
            else matrix.InsertArrayElementAtIndex(i * reorderableVertices.count + index);
        }
    }

    // Remove a row and a column (cross) in a matrix based on the index of an element in the reorderable list.
    public void RemoveMatrixElement(SerializedProperty matrix, int index)
    {
        int i = 0;
        int j = 0;
        while (i < reorderableVertices.count)
        {
            if (i == index && j == 0)
            {
                while (j <= reorderableVertices.count)
                {
                    matrix.DeleteArrayElementAtIndex(i * reorderableVertices.count);
                    ++j;
                }
            }
            else
            {
                matrix.DeleteArrayElementAtIndex(i * reorderableVertices.count + index);
                ++i;
            }
        }
    }

    // Customize the header of the reorderable list.
    private void DrawHeaderCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "Vertices");
    }

    // Customize the appearance of an element inside the reorderable list.
    private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = reorderableVertices.serializedProperty.GetArrayElementAtIndex(index);
        //EditorGUIUtility.labelWidth = 100;
        EditorGUI.PropertyField(position: new Rect(rect.x + 10, rect.y + 3, Screen.width * 0.7f, height: EditorGUIUtility.singleLineHeight),
            property: element, label: new GUIContent("Vertex " + index), includeChildren: true);
    }

    // Add the corresponding row and column in the adjacency and distance matrices when an element is added to the reorderable list.
    private void OnAddCallback(ReorderableList reorderableList)
    {
        int index;
        if (reorderableList.count == 0) index = 0;
        else if (reorderableList.index >= 0) index = reorderableVertices.index;
        else index = reorderableVertices.count - 1;
        reorderableList.serializedProperty.InsertArrayElementAtIndex(index);
        InsertMatrixElement(adjacencyMatrix.FindPropertyRelative("matrix"), index);
        InsertMatrixElement(distanceMatrix.FindPropertyRelative("matrix"), index);
    }

    // Remove the corresponding row and column in the adjacency and distance matrices when an element is removed from the reorderable list.
    private void OnRemoveCallback(ReorderableList reorderableList)
    {
        if (reorderableList.count > 0)
        {
            int index = reorderableList.index >= 0 ? reorderableList.index : reorderableList.count - 1;
            reorderableList.serializedProperty.DeleteArrayElementAtIndex(index);
            RemoveMatrixElement(adjacencyMatrix.FindPropertyRelative("matrix"), index);
            RemoveMatrixElement(distanceMatrix.FindPropertyRelative("matrix"), index);
        }
    }

    // Calculate the proper height for each element to prevent any overlapping in the custom inspector.
    private float ElementHeightCallback(int index)
    {
        float propertyHeight = EditorGUI.GetPropertyHeight(reorderableVertices.serializedProperty.GetArrayElementAtIndex(index), true);
        float spacing = EditorGUIUtility.singleLineHeight / 2;
        return propertyHeight + spacing;
    }
}

/* References:
 * https://sites.google.com/site/tuxnots/gamming/unity3d/unitymakeyourlistsfunctionalwithreorderablelist
 * https://www.sandordaemen.nl/blog/unity-3d-extending-the-editor-part-3/
 */
