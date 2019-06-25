// Camera Path
// Available on the Unity3D Asset Store
// Copyright (c) 2013 Jasper Stocker http://camerapath.jasperstocker.com
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using UnityEngine;
using System.Collections;

//This is a quick C# script demonstrating the functionality you can access via scripting
//It's really quick and dirty though so don't shoot me... :o)
public class ExternalControlExample : MonoBehaviour {
	
	[SerializeField]
	private CameraPathBezierAnimator pathAnimatorA;
	[SerializeField]
	private CameraPathBezierAnimator pathAnimatorB;
	
	private CameraPathBezierAnimator pathAnimator;
	
	void Start()
	{
		if(pathAnimatorA==null)
			return;
		
		pathAnimator = pathAnimatorA;
	}
	
	void OnGUI()
	{
		if(pathAnimator==null)
			return;
		
		if(GUILayout.Button("START")){
			pathAnimator.Play();
		}
		if(GUILayout.Button("PAUSE"))
			pathAnimator.Pause();
		if(GUILayout.Button("STOP"))
			pathAnimator.Stop();
		if(GUILayout.Button("SWITCH")){
			pathAnimator.Stop();
			if(pathAnimator == pathAnimatorA)
				pathAnimator = pathAnimatorB;
			else
				pathAnimator = pathAnimatorA;
			pathAnimator.Play();
		}
		if(GUILayout.Button("JUMP")){
			pathAnimator.Stop();
			pathAnimator.Seek(0.75f);
			pathAnimator.Play();
		}
		if(!pathAnimator.isPlaying){
			if(GUILayout.Button("REPLAY")){
				if(pathAnimator.mode != CameraPathBezierAnimator.modes.reverse)
					pathAnimator.Seek(0);
				else
					pathAnimator.Seek(1);
				pathAnimator.Play();
			}
		}
		
		GUILayout.Space(10.0f);
		GUILayout.Label("ANIMATION MODE");
		GUILayout.Label("current:"+pathAnimator.mode);
		if(GUILayout.Button("FORWARD"))
			pathAnimator.mode = CameraPathBezierAnimator.modes.once;
		if(GUILayout.Button("REVERSE"))
			pathAnimator.mode = CameraPathBezierAnimator.modes.reverse;
		if(GUILayout.Button("LOOP"))
			pathAnimator.mode = CameraPathBezierAnimator.modes.loop;
		
		//Urgh! This is cheap way of doing things - better to keep this value stored somewhere instead of setting it every frame...
		CameraPathBezier bezier = pathAnimator.bezier;
		GUILayout.Space(10.0f);
		GUILayout.Label("CAMERA MODE");
		GUILayout.Label("current:"+bezier.mode);
		if(GUILayout.Button("USER CONTROLLED"))
			bezier.mode = CameraPathBezier.viewmodes.usercontrolled;
		if(GUILayout.Button("MOUSE LOOK"))
			bezier.mode = CameraPathBezier.viewmodes.mouselook;
		if(GUILayout.Button("FOLLOW PATH"))
			bezier.mode = CameraPathBezier.viewmodes.followpath;
		if(GUILayout.Button("REVERSE FOLLOW PATH"))
			bezier.mode = CameraPathBezier.viewmodes.reverseFollowpath;
	}
}
