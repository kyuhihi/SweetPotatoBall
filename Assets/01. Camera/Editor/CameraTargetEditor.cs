using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraTarget))]
public class CameraTargetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CameraTarget cameraTarget = (CameraTarget)target;

        if (GUILayout.Button("Call LateUpdate"))
        {
            cameraTarget.CallLateUpdateManually();
        }
    }
}