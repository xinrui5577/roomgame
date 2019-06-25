// Camera Path
// Available on the Unity3D Asset Store
// Copyright (c) 2013 Jasper Stocker http://camerapath.jasperstocker.com
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using UnityEditor;
using UnityEngine;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(CameraPathBezier))]
public class CameraPathBezierEditor : Editor {
	
	Vector2 scrollPosition;
	private CameraPathBezierControlPoint bezierControlPoint;
	
	public void OnSceneGUI()
	{
		CameraPathBezier bezier = (CameraPathBezier)target;
		
		if(GUI.changed){
			bezier.RecalculateStoredValues();
            EditorUtility.SetDirty(bezier);
		}
	}
	
	public override void OnInspectorGUI()
	{
		CameraPathBezier bezier = (CameraPathBezier)target;
		int numberOfControlPoints = bezier.numberOfControlPoints;
		
		if(numberOfControlPoints>0)
		{
			if(numberOfControlPoints>1){
				GUILayout.Space(10);
				bezier.lineColour = EditorGUILayout.ColorField("Line Colour",bezier.lineColour);
				GUILayout.Space(5);
				
				bezier.mode = (CameraPathBezier.viewmodes)EditorGUILayout.EnumPopup("Camera Mode", bezier.mode);
				GUILayout.Space(5);
				
				if(bezier.mode == CameraPathBezier.viewmodes.target){
					if(bezier.target==null)
						EditorGUILayout.HelpBox("No target has been specified in the bezier path",MessageType.Warning);
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Look at Target");
					bezier.target = (Transform)EditorGUILayout.ObjectField(bezier.target, typeof(Transform),true);
					EditorGUILayout.EndHorizontal();
					GUILayout.Space(5);
				}
				
				bool newValue = EditorGUILayout.Toggle("Loop",bezier.loop);
				if(newValue != bezier.loop){
					Undo.RegisterUndo(bezier,"Set Loop");
					bezier.loop = newValue;
					bezier.RecalculateStoredValues();
				}
				
			}else{
				if(numberOfControlPoints==1)
					EditorGUILayout.HelpBox("Click Add New Point to add additional points, you need at least two points to make a path.",MessageType.Info);
			}
			
			GUILayout.Space(5);
			if(GUILayout.Button("Reset Path")){
				if(EditorUtility.DisplayDialog("Resetting path?", "Are you sure you want to delete all control points?", "Delete", "Cancel")){
					Undo.RegisterSceneUndo("Reset Camera Path");
					bezier.ResetPath();
					return;
				}
			}
			
			GUILayout.Space(10);
			GUILayout.Box(EditorGUIUtility.whiteTexture, GUILayout.Height(2), GUILayout.Width(Screen.width-20));
			GUILayout.Space(3);
			
			//scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			for(int i=0; i<numberOfControlPoints; i++)
			{
				CameraPathBezierControlPoint go = bezier.controlPoints[i];
				
				if(go==null)
					go = (CameraPathBezierControlPoint)EditorGUILayout.ObjectField(null,typeof( CameraPathBezierControlPoint ), true);
				else
					go = (CameraPathBezierControlPoint)EditorGUILayout.ObjectField( go.name, go, typeof( CameraPathBezierControlPoint ), true);
				
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("Delete")){
					Undo.RegisterSceneUndo("Delete Camera Path point");
					bezier.DeletePoint(i,true);
					numberOfControlPoints = bezier.numberOfControlPoints;
					EditorUtility.SetDirty(bezier);
					return;	
				}
				
				if(i==numberOfControlPoints-1){
					if(GUILayout.Button("Add New Point At End")){
						Undo.RegisterSceneUndo("Create a new Camera Path point");
						bezier.AddNewPoint(i+1);
						EditorUtility.SetDirty(bezier);
					}
				}else{
					if(GUILayout.Button("Add New Point Between")){
						Undo.RegisterSceneUndo("Create a new Camera Path point");
						bezier.AddNewPoint(i+1);
						EditorUtility.SetDirty(bezier);
					}
				}
				EditorGUILayout.EndHorizontal();
				
				GUILayout.Space(7);
				GUILayout.Box(EditorGUIUtility.whiteTexture, GUILayout.Height(2), GUILayout.Width(Screen.width-25));
				GUILayout.Space(7);
			} 
			//EditorGUILayout.EndScrollView();
		}else{
			if(GUILayout.Button("Add New Point At End"))
			{
				Undo.RegisterSceneUndo("Create a new Camera Path point");
				bezier.AddNewPoint();
				EditorUtility.SetDirty(bezier);
			}
			EditorGUILayout.HelpBox("Click Add New Point to add points and begin the path, you will need two points to create a path.",MessageType.Info);
		}
		
		if(GUI.changed)
		{
			bezier.RecalculateStoredValues();
            EditorUtility.SetDirty(bezier);
		}
	}
	
}
