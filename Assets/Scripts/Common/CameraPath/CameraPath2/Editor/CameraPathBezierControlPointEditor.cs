// Camera Path
// Available on the Unity3D Asset Store
// Copyright (c) 2013 Jasper Stocker http://camerapath.jasperstocker.com
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(CameraPathBezierControlPoint))]
public class CameraPathBezierControlPointEditor : Editor
{

    private CameraPathBezierControlPoint bezierControlPoint;
    private CameraPathBezierAnimator animator;
    private CameraPathBezier bezier;

    private RenderTexture pointPreviewTexture = null;
    private float aspect = 1.7777f;
    private Camera sceneCamera = null;
    private Skybox sceneCameraSkybox = null;

    private Vector3 previewCamPos;
    private Quaternion previewCamRot;
    private float previewCamFOV;

    void OnEnable()
    {
        bezierControlPoint = (CameraPathBezierControlPoint)target;
        bezier = bezierControlPoint.bezier;
        animator = bezier.GetComponent<CameraPathBezierAnimator>();
    }

    void OnSceneGUI()
    {

        Vector3 newPosition = bezierControlPoint.worldControlPoint;// bezierControlPoint.controlPoint + bezierControlPoint.transform.position;
        float handlesize = HandleUtility.GetHandleSize(newPosition) * 0.6f;

        Handles.color = new Color(0.99f, 0.50f, 0.35f);
        newPosition = Handles.Slider(newPosition, Vector3.right, handlesize, Handles.ArrowCap, 1);
        Handles.color = new Color(0.30f, 0.85f, 0.99f);
        newPosition = Handles.Slider(newPosition, Vector3.forward, handlesize, Handles.ArrowCap, 1);
        Handles.color = new Color(0.85f, 0.95f, 0.30f);
        newPosition = Handles.Slider(newPosition, Vector3.up, handlesize, Handles.ArrowCap, 1);

        //Tilting draw function for follow path
        if (bezier.mode == CameraPathBezier.viewmodes.followpath || bezier.mode == CameraPathBezier.viewmodes.reverseFollowpath)
        {
            Handles.color = Color.yellow;
            float tiltScale = HandleUtility.GetHandleSize(bezierControlPoint.transform.position) * 1.25f;
            Vector3 forward = bezierControlPoint.GetPathDirection().normalized * tiltScale;
            Quaternion fwdRot = Quaternion.LookRotation(forward);
            Vector3 tilt = Quaternion.Euler(0, 0, -bezierControlPoint.tilt) * (Vector3.up * tiltScale);
            Handles.DrawWireDisc(bezierControlPoint.transform.position, forward, tiltScale);
            Handles.DrawLine(bezierControlPoint.transform.position, bezierControlPoint.transform.position + fwdRot * tilt);
        }


        Handles.color = Color.red;
        Handles.DrawLine(bezierControlPoint.transform.position, bezierControlPoint.worldControlPoint);

        if (bezierControlPoint.worldControlPoint != newPosition)
        {
            GUI.changed = true;
            bezierControlPoint.worldControlPoint = newPosition;
        }

        Handles.Label(bezierControlPoint.transform.position + Vector3.up * HandleUtility.GetHandleSize(newPosition) * 2f, bezierControlPoint.name);

        Handles.color = (Color.white - bezier.lineColour) + new Color(0, 0, 0, 1);
        Handles.ArrowCap(0, previewCamPos, previewCamRot, handlesize * 1.5f);

        if (GUI.changed)
        {
            bezier.RecalculateStoredValues();
            Undo.RegisterUndo(bezierControlPoint, "Move Control Point");
            EditorUtility.SetDirty(bezierControlPoint);
            EditorUtility.SetDirty(bezier);
            EditorUtility.SetDirty(animator);
        }
    }

    public override void OnInspectorGUI()
    {
        Camera[] cams = Camera.allCameras;
        bool sceneHasCamera = cams.Length > 0;
        if (Camera.main)
        {
            sceneCamera = Camera.main;
        }
        else if (sceneHasCamera)
        {
            sceneCamera = cams[0];
        }

        if (sceneCamera != null)
            if (sceneCameraSkybox == null)
                sceneCameraSkybox = sceneCamera.GetComponent<Skybox>();

        if (pointPreviewTexture == null)
            pointPreviewTexture = new RenderTexture(400, Mathf.RoundToInt(400 / aspect), 24);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name:", GUILayout.Width(50));
        bezierControlPoint.name = EditorGUILayout.TextField(bezierControlPoint.name);
        EditorGUILayout.EndHorizontal();

        if ((EditorGUIUtility.isProSkin) && bezier.numberOfCurves > 0 && pointPreviewTexture != null)
        {

            bool cameraPathPreview = EditorPrefs.GetBool("CameraPathPreview");
            GUILayout.Space(7);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Point Preview");
            if (cameraPathPreview)
            {
                if (GUILayout.Button("Hide", GUILayout.Width(50)))
                    EditorPrefs.SetBool("CameraPathPreview", false);
            }
            else
            {
                if (GUILayout.Button("Show", GUILayout.Width(50)))
                    EditorPrefs.SetBool("CameraPathPreview", true);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            if (!Application.isPlaying && EditorPrefs.GetBool("CameraPathPreview"))
            {
                if (animator.animationTarget == null)
                {
                    EditorGUILayout.HelpBox("No animation target has been specified so there is nothing to animate. Select an animation target in the Camera Path Bezier Animator Component in the parent clip", MessageType.Warning);
                    return;
                }

                previewCamPos = bezierControlPoint.transform.position;
                previewCamRot = Quaternion.identity;
                previewCamFOV = bezierControlPoint.FOV;

                Vector3 plusPoint, minusPoint;
                float pointPercentage = bezier.GetPathPercentageAtPoint(bezierControlPoint);
                switch (bezier.mode)
                {
                    case CameraPathBezier.viewmodes.usercontrolled:
                        previewCamRot = bezierControlPoint.transform.rotation;
                        break;

                    case CameraPathBezier.viewmodes.target:

                        if (bezier.target != null)
                        {
                            previewCamRot = Quaternion.LookRotation(bezier.target.transform.position - bezierControlPoint.transform.position);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("No target has been specified in the bezier path", MessageType.Warning);
                            previewCamRot = Quaternion.identity;
                        }
                        break;

                    case CameraPathBezier.viewmodes.followpath:

                        minusPoint = bezier.GetPathPosition(Mathf.Clamp01(pointPercentage - 0.05f));
                        plusPoint = bezier.GetPathPosition(Mathf.Clamp01(pointPercentage + 0.05f));
                        previewCamRot = Quaternion.LookRotation(plusPoint - minusPoint);
                        break;

                    case CameraPathBezier.viewmodes.reverseFollowpath:

                        minusPoint = bezier.GetPathPosition(Mathf.Clamp01(pointPercentage - 0.05f));
                        plusPoint = bezier.GetPathPosition(Mathf.Clamp01(pointPercentage + 0.05f));
                        previewCamRot = Quaternion.LookRotation(minusPoint - plusPoint);
                        break;

                    case CameraPathBezier.viewmodes.mouselook:

                        Vector3 minusPointb = bezier.GetPathPosition(Mathf.Clamp01(pointPercentage - 0.05f));
                        Vector3 plusPointb = bezier.GetPathPosition(Mathf.Clamp01(pointPercentage + 0.05f));
                        previewCamRot = Quaternion.LookRotation(plusPointb - minusPointb);
                        break;
                }

                GameObject cam = new GameObject("Point Preview");
                cam.AddComponent<Camera>();
                if (sceneCamera != null)
                {
                    cam.GetComponent<Camera>().backgroundColor = sceneCamera.backgroundColor;
                    if (sceneCameraSkybox != null)
                        cam.AddComponent<Skybox>().material = sceneCameraSkybox.material;
                    else
                        if (RenderSettings.skybox != null)
                            cam.AddComponent<Skybox>().material = RenderSettings.skybox;
                }
                cam.transform.position = previewCamPos;
                cam.transform.rotation = previewCamRot;
                cam.GetComponent<Camera>().fov = previewCamFOV;
                cam.GetComponent<Camera>().targetTexture = pointPreviewTexture;
                cam.GetComponent<Camera>().Render();
                cam.GetComponent<Camera>().targetTexture = null;

                DestroyImmediate(cam);

                Rect previewRect = new Rect(0, 0, Screen.width, Screen.width / aspect);
                Rect layoutRect = EditorGUILayout.BeginVertical();
                previewRect.x = layoutRect.x;
                previewRect.y = layoutRect.y + 5;
                EditorGUI.DrawPreviewTexture(previewRect, pointPreviewTexture);
                GUILayout.Space(previewRect.height + 10);
                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("The control point modifies the curve. Resetting it will centre it and there will be no curve. Useful if you don't want there to be a curve at this point", MessageType.Info);
        if (GUILayout.Button("Reset Control Point"))
        {
            Undo.RegisterUndo(bezierControlPoint.gameObject, "Reset Control Point");
            bezierControlPoint.controlPoint = Vector3.zero;
            EditorUtility.SetDirty(bezierControlPoint);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("Make the rotation of the Control Point face the direction the path is going in at this point", MessageType.Info);
        if (GUILayout.Button("Face Path Direction"))
        {
            Undo.RegisterUndo(bezierControlPoint.gameObject.transform, "Set Control Point Rotation to Path Direction");
            bezierControlPoint.SetRotationToCurve();
            EditorUtility.SetDirty(bezierControlPoint);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        if (!bezierControlPoint.isLastPoint || bezier.loop)
        {
            EditorGUILayout.HelpBox("This controls the easing applied from this point to the next.", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Animation Ease");
            bezierControlPoint.ease = (CameraPathBezierControlPoint.animationEase)EditorGUILayout.EnumPopup(bezierControlPoint.ease);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("The last control point does not have easing options as there is no following curve.", MessageType.Info);
        }

        bezierControlPoint.controlPoint = EditorGUILayout.Vector3Field("Control Point Location", bezierControlPoint.controlPoint);
        //
        GUILayout.Space(7);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Field of View");
        bezierControlPoint.FOV = EditorGUILayout.Slider(bezierControlPoint.FOV, 1, 180);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(7);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Delay");
        bezierControlPoint.delayMode = (CameraPathBezierControlPoint.DELAY_MODES)EditorGUILayout.EnumPopup(bezierControlPoint.delayMode);
        EditorGUILayout.EndHorizontal();

        if (bezierControlPoint.delayMode == CameraPathBezierControlPoint.DELAY_MODES.timed)
        {
            GUILayout.Space(7);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time of delay");
            bezierControlPoint.delayTime = EditorGUILayout.FloatField(bezierControlPoint.delayTime, GUILayout.Width(50));
            EditorGUILayout.LabelField("secs", GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();
        }

        if (bezier.mode == CameraPathBezier.viewmodes.followpath || bezier.mode == CameraPathBezier.viewmodes.reverseFollowpath)
        {
            GUILayout.Space(7);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tilt", GUILayout.Width(40));
            bezierControlPoint.tilt = EditorGUILayout.Slider(bezierControlPoint.tilt, -180, 180);
            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            bezier.RecalculateStoredValues();

            EditorUtility.SetDirty(bezierControlPoint);
            EditorUtility.SetDirty(bezier);
            EditorUtility.SetDirty(animator);
        }
    }
}
