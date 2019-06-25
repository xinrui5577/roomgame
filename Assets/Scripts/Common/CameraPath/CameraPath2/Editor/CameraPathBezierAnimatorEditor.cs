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
[CustomEditor(typeof(CameraPathBezierAnimator))]
public class CameraPathBezierAnimatorEditor : Editor {
	
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
		animator = (CameraPathBezierAnimator)target;
		bezier = animator.GetComponent<CameraPathBezier>();
	}
	
	public void OnSceneGUI()
	{
		
		if(animator.showScenePreview)
		{		
			float handleSize = HandleUtility.GetHandleSize(previewCamPos * 0.5f);
			Handles.color = (Color.white - bezier.lineColour) + new Color(0,0,0,1);
			Handles.DrawLine(previewCamPos,previewCamPos+Vector3.up * 0.5f);
			Handles.DrawLine(previewCamPos,previewCamPos+Vector3.down * 0.5f);
			Handles.DrawLine(previewCamPos,previewCamPos+Vector3.left * 0.5f);
			Handles.DrawLine(previewCamPos,previewCamPos+Vector3.right * 0.5f);
			Handles.DrawLine(previewCamPos,previewCamPos+Vector3.forward * 0.5f);
			Handles.DrawLine(previewCamPos,previewCamPos+Vector3.back * 0.5f);
			
			Handles.ArrowCap(0, previewCamPos, previewCamRot, handleSize);
			Handles.Label(previewCamPos, "Preview\nCamera\nPosition");
		}
		
		if(GUI.changed)
		{
			bezier.RecalculateStoredValues();
			EditorUtility.SetDirty (animator);
			EditorUtility.SetDirty (bezier);
		}
	}
	
	public override void OnInspectorGUI()
	{
		Camera[] cams = Camera.allCameras;
		bool sceneHasCamera = cams.Length>0;
		if(Camera.main){
			sceneCamera = Camera.main;
		}else if(sceneHasCamera){
			sceneCamera = cams[0];
		}
		
		if(sceneCamera!=null)
			if(sceneCameraSkybox==null)
				sceneCameraSkybox = sceneCamera.GetComponent<Skybox>();
		
		if(pointPreviewTexture==null)
			pointPreviewTexture = new RenderTexture(400, Mathf.RoundToInt(400/aspect), 24);
		
		if(animator.animationTarget==null){
			EditorGUILayout.HelpBox("No animation target has been specified so there is nothing to animate. Select an animation target in the Camera Path Bezier Animator Component in the parent clip",MessageType.Warning);
		}else{
			if((EditorGUIUtility.isProSkin) && bezier.numberOfCurves > 0 && pointPreviewTexture!=null){
				
				bool cameraPathPreview = EditorPrefs.GetBool("CameraPathPreview");
				GUILayout.Space(7);
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Animation Preview");
				if(cameraPathPreview)
				{
					if(GUILayout.Button("Hide", GUILayout.Width(50)))
						EditorPrefs.SetBool("CameraPathPreview", false);
				}else{
					if(GUILayout.Button("Show", GUILayout.Width(50)))
						EditorPrefs.SetBool("CameraPathPreview", true);
				}
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				
				if(!Application.isPlaying && cameraPathPreview){
					float usePercentage = animator.normalised? animator.RecalculatePercentage(animator.editorTime): animator.editorTime;
					
					//Get animation values and apply them to the preview camera
					previewCamPos = bezier.GetPathPosition(usePercentage);
					previewCamRot = Quaternion.identity;
					previewCamFOV = bezier.GetPathFOV(usePercentage);
					
					//Assign rotation to preview camera
					Vector3 plusPoint, minusPoint;
					switch(bezier.mode)
					{
					case CameraPathBezier.viewmodes.usercontrolled:
						previewCamRot = bezier.GetPathRotation(usePercentage);
						break;
						
					case CameraPathBezier.viewmodes.target:
						
						if(bezier.target != null){
							previewCamRot = Quaternion.LookRotation(bezier.target.transform.position - previewCamPos);
						}else{
							EditorGUILayout.HelpBox("No target has been specified in the bezier path",MessageType.Warning);
							previewCamRot = Quaternion.identity;
						}
						break;
						
					case CameraPathBezier.viewmodes.followpath:
						
						minusPoint = bezier.GetPathPosition(Mathf.Clamp01(usePercentage-0.05f));
						plusPoint = bezier.GetPathPosition(Mathf.Clamp01(usePercentage+0.05f));
						previewCamRot = Quaternion.LookRotation(plusPoint-minusPoint);
						break;
						
					case CameraPathBezier.viewmodes.reverseFollowpath:
						
						minusPoint = bezier.GetPathPosition(Mathf.Clamp01(usePercentage-0.05f));
						plusPoint = bezier.GetPathPosition(Mathf.Clamp01(usePercentage+0.05f));
						previewCamRot = Quaternion.LookRotation(minusPoint-plusPoint);
						break;
						
					case CameraPathBezier.viewmodes.mouselook:
						
						Vector3 minusPointb = bezier.GetPathPosition(Mathf.Clamp01(usePercentage-0.05f));
						Vector3 plusPointb = bezier.GetPathPosition(Mathf.Clamp01(usePercentage+0.05f));
						previewCamRot = Quaternion.LookRotation(plusPointb-minusPointb);
						break;
					}
					
					//Render the camera preview
					GameObject cam = new GameObject("Point Preview");
					cam.transform.parent = bezier.transform;
					cam.AddComponent<Camera>();
					//Retreive camera settings from the main camera
					if(sceneCamera!=null){
						cam.GetComponent<Camera>().backgroundColor = sceneCamera.backgroundColor;
						if(sceneCameraSkybox!=null)
							cam.AddComponent<Skybox>().material = sceneCameraSkybox.material;
						else
							if(RenderSettings.skybox!=null)
								cam.AddComponent<Skybox>().material = RenderSettings.skybox;
					}
					cam.transform.position = previewCamPos;
					cam.transform.rotation = previewCamRot;
					cam.GetComponent<Camera>().fov = previewCamFOV;
					cam.GetComponent<Camera>().targetTexture = pointPreviewTexture;
			        cam.GetComponent<Camera>().Render();
			        cam.GetComponent<Camera>().targetTexture = null;
					DestroyImmediate(cam);
					
					//Display the camera preview
					
					Rect previewRect = new Rect(0,0, Screen.width, Screen.width/aspect);
					Rect layoutRect = EditorGUILayout.BeginVertical();
					previewRect.x = layoutRect.x;
					previewRect.y = layoutRect.y+5;
					EditorGUI.DrawPreviewTexture(previewRect, pointPreviewTexture);
					GUILayout.Space(previewRect.height+10);
					pointPreviewTexture.Release();
					
					EditorGUILayout.BeginHorizontal();
					float time = EditorGUILayout.Slider(animator.editorTime*animator.pathTime,0,animator.pathTime);
					animator.editorTime = time/animator.pathTime;
					EditorGUILayout.LabelField("sec",GUILayout.Width(25));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}
			}
		}
		
		animator.showScenePreview = EditorGUILayout.Toggle("Show Scene Preview Info", animator.showScenePreview);
		
		animator.playOnStart = EditorGUILayout.Toggle("Play on start", animator.playOnStart);
		
		EditorGUILayout.BeginHorizontal();
		animator.pathTime = EditorGUILayout.FloatField("Animation Time",animator.pathTime);
		EditorGUILayout.LabelField("sec",GUILayout.Width(25));
		EditorGUILayout.EndHorizontal();
		
		bool noPath = bezier.numberOfControlPoints<2;
		EditorGUI.BeginDisabledGroup(noPath);
		EditorGUILayout.BeginHorizontal();
		float newPathSpeed = EditorGUILayout.FloatField("Animation Speed",animator.pathSpeed);
		if(!noPath)
			animator.pathSpeed = newPathSpeed;
		EditorGUILayout.LabelField("m/sec",GUILayout.Width(25));
		EditorGUILayout.EndHorizontal();
		EditorGUI.EndDisabledGroup();
		
		animator.pathTime = Mathf.Max(animator.pathTime,0.001f);//ensure it's a real number
		
		animator.animationTarget = (Transform)EditorGUILayout.ObjectField("Animate Object",animator.animationTarget, typeof(Transform),true);
		EditorGUILayout.HelpBox("This toggle can be used to specify what kind of object you are animating. If it isn't a camera, we recommend you uncheck this box",MessageType.Info);
		animator.isCamera = EditorGUILayout.Toggle("Is Camera", animator.isCamera);
		
		animator.mode = (CameraPathBezierAnimator.modes)EditorGUILayout.EnumPopup("Animation Mode",animator.mode);
		
		animator.normalised = EditorGUILayout.Toggle("Normalised Path",animator.normalised);
		
		EditorGUILayout.HelpBox("Set this if you want to start another camera path animation once this has completed",MessageType.Info);
		animator.nextAnimation = (CameraPathBezierAnimator)EditorGUILayout.ObjectField("Next Camera Path",animator.nextAnimation, typeof(CameraPathBezierAnimator),true);
		
		if(bezier.mode == CameraPathBezier.viewmodes.mouselook)
		{
			EditorGUILayout.HelpBox("Alter the mouse sensitivity here",MessageType.Info);
			animator.sensitivity = EditorGUILayout.Slider("Mouse Sensitivity",animator.sensitivity,0.1f, 2.0f);
			EditorGUILayout.HelpBox("Restrict the vertical viewable area here.",MessageType.Info);
			EditorGUILayout.LabelField("Mouse Y Restriction");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(((int)animator.minX).ToString(),GUILayout.Width(30));
			EditorGUILayout.MinMaxSlider(ref animator.minX, ref animator.maxX, -180, 180);
			EditorGUILayout.LabelField(((int)animator.maxX).ToString(),GUILayout.Width(30));
			EditorGUILayout.EndHorizontal();
		}
		
        if (GUI.changed)
		{
			bezier.RecalculateStoredValues();
			EditorUtility.SetDirty (animator);
			EditorUtility.SetDirty (bezier);
		}
	}	
}
