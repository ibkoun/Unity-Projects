using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomSpherePackingScript)), CanEditMultipleObjects]
public class RandomSpherePackingEditor : Editor
{
    private SerializedProperty centerpoint, size, numberOfSpheres, randomRadius, radius, minRadius, maxRadius, randomDistance, distance, minDistance, maxDistance, maxIterations;

    private void OnEnable()
    {
        centerpoint = serializedObject.FindProperty("centerpoint");
        size = serializedObject.FindProperty("size");
        numberOfSpheres = serializedObject.FindProperty("numberOfSpheres");
        randomRadius = serializedObject.FindProperty("randomRadius");
        radius = serializedObject.FindProperty("radius");
        minRadius = serializedObject.FindProperty("minRadius");
        maxRadius = serializedObject.FindProperty("maxRadius");
        randomDistance = serializedObject.FindProperty("randomDistance");
        distance = serializedObject.FindProperty("distance");
        minDistance = serializedObject.FindProperty("minDistance");
        maxDistance = serializedObject.FindProperty("maxDistance");
        maxIterations = serializedObject.FindProperty("maxIterations");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        // Bounding box parameters.
        EditorGUILayout.LabelField("Bounding box parameters", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(centerpoint, new GUIContent("Centerpoint", "Center point of the box."));
        EditorGUILayout.PropertyField(size, new GUIContent("Size", "Size of the box."));
        EditorGUI.indentLevel--;

        // Sphere parameters.
        EditorGUILayout.LabelField("Sphere parameters", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(numberOfSpheres, new GUIContent("Number of spheres", "Maximum number of spheres allowed inside the box."));
        EditorGUILayout.PropertyField(randomRadius, new GUIContent("Random radius", "If true, each sphere will have a random radius."));
        EditorGUI.BeginDisabledGroup(randomRadius.boolValue);
        EditorGUILayout.PropertyField(radius, new GUIContent("Radius", "Radius for every sphere."));
        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(!randomRadius.boolValue);
        EditorGUILayout.PropertyField(minRadius, new GUIContent("Min radius", "Lower bound for the radius of a sphere to be randomized"));
        EditorGUILayout.PropertyField(maxRadius, new GUIContent("Max radius", "Upper bound for the radius of a sphere to be randomized"));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(randomDistance, new GUIContent("Random distance", "If true, randomize the minimum distance between each sphere."));
        EditorGUI.BeginDisabledGroup(randomDistance.boolValue);
        EditorGUILayout.PropertyField(distance, new GUIContent("Distance", "Minimum distance between each sphere."));
        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(!randomDistance.boolValue);
        EditorGUILayout.PropertyField(minDistance, new GUIContent("Min distance", "Lower bound for the mininum distance to be randomized."));
        EditorGUILayout.PropertyField(maxDistance, new GUIContent("Max distance", "Upper bound for the minimum distance to be randomized."));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(maxIterations, new GUIContent("Max iterations", "Maximum number of attempts to fit a sphere inside the box."));
        EditorGUI.indentLevel--;

        if (GUILayout.Button("Create random spheres"))
        {
            ((RandomSpherePackingScript)target).CreateRandomSpheres();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

/* References:
 * https://answers.unity.com/questions/682932/using-generic-list-with-serializedproperty-inspect.html
 */
