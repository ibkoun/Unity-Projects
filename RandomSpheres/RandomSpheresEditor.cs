using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomSpheresScript)), CanEditMultipleObjects]
public class RandomSpheresEditor : Editor
{
    private SerializedProperty center, size, numberOfSpheres, randomRadius, radius, minRadius, maxRadius, maxIterations;

    private void OnEnable()
    {
        center = serializedObject.FindProperty("center");
        size = serializedObject.FindProperty("size");
        numberOfSpheres = serializedObject.FindProperty("numberOfSpheres");
        randomRadius = serializedObject.FindProperty("randomRadius");
        radius = serializedObject.FindProperty("radius");
        minRadius = serializedObject.FindProperty("minRadius");
        maxRadius = serializedObject.FindProperty("maxRadius");
        maxIterations = serializedObject.FindProperty("maxIterations");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        // Bounding box parameters.
        EditorGUILayout.LabelField("Bounding box parameters", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(center, new GUIContent("Center", "Center point of the box."));
        EditorGUILayout.PropertyField(size, new GUIContent("Size", "Size of the box."));
        EditorGUI.indentLevel--;

        // Sphere parameters.
        EditorGUILayout.LabelField("Sphere parameters", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(numberOfSpheres, new GUIContent("Number of spheres", "Max number of spheres allowed inside the box."));
        EditorGUILayout.PropertyField(randomRadius, new GUIContent("Random radius", "If true, randomize the radius of each sphere."));
        EditorGUI.BeginDisabledGroup(randomRadius.boolValue);
        EditorGUILayout.PropertyField(radius, new GUIContent("Radius"));
        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(!randomRadius.boolValue);
        EditorGUILayout.PropertyField(minRadius, new GUIContent("Min radius"));
        EditorGUILayout.PropertyField(maxRadius, new GUIContent("Max radius"));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(maxIterations, new GUIContent("Max iterations", "The number of attempts to fit a sphere inside the box."));
        EditorGUI.indentLevel--;

        if (GUILayout.Button("Create random spheres"))
        {
            ((RandomSpheresScript)target).CreateRandomSpheres();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

/* References:
 * https://answers.unity.com/questions/682932/using-generic-list-with-serializedproperty-inspect.html
 */
