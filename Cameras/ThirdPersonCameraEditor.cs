using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ThirdPersonCameraScript)), CanEditMultipleObjects]
public class ThirdPersonCameraEditor : Editor
{
    private SerializedProperty targetObject, offset, movementSpeed, movementSmoothness,
        rotationSpeed, rotationSmoothness,
        rotationConstraints,
        pitchConstraints, freezePitch, clampPitch, minPitchValue, maxPitchValue,
        yawConstraints, freezeYaw, clampYaw, minYawValue, maxYawValue,
        rollConstraints, freezeRoll, clampRoll, minRollValue, maxRollValue;
    private bool showMovementParams, showRotationParams;

    private void OnEnable()
    {
        targetObject = serializedObject.FindProperty("targetObject");
        offset = serializedObject.FindProperty("offset");

        // Translations
        movementSpeed = serializedObject.FindProperty("movementSpeed");
        movementSmoothness = serializedObject.FindProperty("movementSmoothness");

        // Rotations
        rotationSpeed = serializedObject.FindProperty("rotationSpeed");
        rotationSmoothness = serializedObject.FindProperty("rotationSmoothness");
        rotationConstraints = serializedObject.FindProperty("rotationConstraints");

        // Pitch
        pitchConstraints = rotationConstraints.FindPropertyRelative("X");
        freezePitch = pitchConstraints.FindPropertyRelative("freeze");
        clampPitch = pitchConstraints.FindPropertyRelative("clamp");
        minPitchValue = pitchConstraints.FindPropertyRelative("minValue");
        maxPitchValue = pitchConstraints.FindPropertyRelative("maxValue");

        // Yaw
        yawConstraints = rotationConstraints.FindPropertyRelative("Y");
        freezeYaw = yawConstraints.FindPropertyRelative("freeze");
        clampYaw = yawConstraints.FindPropertyRelative("clamp");
        minYawValue = yawConstraints.FindPropertyRelative("minValue");
        maxYawValue = yawConstraints.FindPropertyRelative("maxValue");

        // Roll
        rollConstraints = rotationConstraints.FindPropertyRelative("Z");
        freezeRoll = rollConstraints.FindPropertyRelative("freeze");
        clampRoll = rollConstraints.FindPropertyRelative("clamp");
        minRollValue = rollConstraints.FindPropertyRelative("minValue");
        maxRollValue = rollConstraints.FindPropertyRelative("maxValue");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Target setting", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(targetObject, new GUIContent("Target", "The game object the camera is looking at."));
        EditorGUILayout.PropertyField(offset, new GUIContent("Offset", "The position of the camera relative to the position of the game object."));
        EditorGUI.indentLevel--
            ;
        // Translations
        showMovementParams = EditorGUILayout.Foldout(showMovementParams, "Movement parameters");
        EditorGUI.indentLevel++;
        if (showMovementParams)
        {
            movementSpeed.floatValue = EditorGUILayout.Slider("Speed", movementSpeed.floatValue, 1, 100);
            movementSmoothness.floatValue = EditorGUILayout.Slider("Smoothness", movementSmoothness.floatValue, 1, 100);
        }
        EditorGUI.indentLevel--;

        // Rotations
        showRotationParams = EditorGUILayout.Foldout(showRotationParams, "Rotation parameters");
        EditorGUI.indentLevel++;
        if (showRotationParams)
        {
            rotationSpeed.floatValue = EditorGUILayout.Slider("Speed", rotationSpeed.floatValue, 1, 100);
            rotationSmoothness.floatValue = EditorGUILayout.Slider("Smoothness", rotationSmoothness.floatValue, 1, 100);

            // Rotation constraints
            rotationConstraints.isExpanded = EditorGUILayout.Foldout(rotationConstraints.isExpanded, "Constraints");
            EditorGUI.indentLevel++;
            if (rotationConstraints.isExpanded)
            {
                // Pitch
                pitchConstraints.isExpanded = EditorGUILayout.Foldout(pitchConstraints.isExpanded, new GUIContent("Pitch", "Rotation around the x-axis."));
                EditorGUI.indentLevel++;
                if (pitchConstraints.isExpanded)
                {
                    freezePitch.boolValue = EditorGUILayout.Toggle(new GUIContent("Freeze", "Prevent the rotation around the x-axis."), freezePitch.boolValue);
                    EditorGUI.BeginDisabledGroup(freezePitch.boolValue);
                    clampPitch.boolValue = EditorGUILayout.Toggle(new GUIContent("Clamp", "Limit the rotation around the x-axis."), clampPitch.boolValue);
                    EditorGUI.BeginDisabledGroup(!clampPitch.boolValue);
                    minPitchValue.floatValue = EditorGUILayout.Slider("Min value", minPitchValue.floatValue, -360, 360);
                    maxPitchValue.floatValue = EditorGUILayout.Slider("Max value", maxPitchValue.floatValue, -360, 360);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.indentLevel--;

                // Yaw
                yawConstraints.isExpanded = EditorGUILayout.Foldout(yawConstraints.isExpanded, new GUIContent("Yaw", "Rotation around the y-axis."));
                EditorGUI.indentLevel++;
                if (yawConstraints.isExpanded)
                {
                    freezeYaw.boolValue = EditorGUILayout.Toggle(new GUIContent("Freeze", "Prevent the rotation around the y-axis."), freezeYaw.boolValue);
                    EditorGUI.BeginDisabledGroup(freezeYaw.boolValue);
                    clampYaw.boolValue = EditorGUILayout.Toggle(new GUIContent("Clamp", "Limit the rotation around the y-axis."), clampYaw.boolValue);
                    EditorGUI.BeginDisabledGroup(!clampYaw.boolValue);
                    minYawValue.floatValue = EditorGUILayout.Slider("Min value", minYawValue.floatValue, -360, 360);
                    maxYawValue.floatValue = EditorGUILayout.Slider("Max value", maxYawValue.floatValue, -360, 360);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.indentLevel--;

                // Roll
                rollConstraints.isExpanded = EditorGUILayout.Foldout(rollConstraints.isExpanded, new GUIContent("Roll", "Rotation around the z-axis."));
                EditorGUI.indentLevel++;
                if (rollConstraints.isExpanded)
                {
                    freezeRoll.boolValue = EditorGUILayout.Toggle(new GUIContent("Freeze", "Prevent the rotation around the z-axis."), freezeRoll.boolValue);
                    EditorGUI.BeginDisabledGroup(freezeRoll.boolValue);
                    clampRoll.boolValue = EditorGUILayout.Toggle(new GUIContent("Clamp", "Limit the rotation around the z-axis."), clampRoll.boolValue);
                    EditorGUI.BeginDisabledGroup(!clampRoll.boolValue);
                    minRollValue.floatValue = EditorGUILayout.Slider("Min value", minRollValue.floatValue, -360, 360);
                    maxRollValue.floatValue = EditorGUILayout.Slider("Max value", maxRollValue.floatValue, -360, 360);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.indentLevel--;
            }

        }
        EditorGUI.indentLevel--;
        serializedObject.ApplyModifiedProperties();
    }
}
