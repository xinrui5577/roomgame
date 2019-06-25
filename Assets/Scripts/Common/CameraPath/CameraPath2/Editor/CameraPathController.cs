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

public class CameraPathController : EditorWindow {

	 [MenuItem("GameObject/Create New Camera Path",false,3)]
    public static void CreatePath()
    {
		GameObject newPath = new GameObject("New Camera Path");
		CameraPathBezierAnimator animator = newPath.AddComponent<CameraPathBezierAnimator>();
		newPath.AddComponent<CameraPathBezier>();
		
		if(Camera.main)
			animator.animationTarget = Camera.main.transform;
	}
}
