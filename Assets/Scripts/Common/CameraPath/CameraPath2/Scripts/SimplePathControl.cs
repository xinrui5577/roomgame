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

public class SimplePathControl : MonoBehaviour {
	
	private CameraPathBezierAnimator animator;
	
	void Start () {
		animator = GetComponent<CameraPathBezierAnimator>();
	}
	
	void Update () {
		if(Input.GetMouseButtonUp(0)){
			if(animator.isPlaying)
				animator.Pause();
			else
				animator.Play();
		}
		if(Input.GetMouseButtonUp(1)){
			animator.Seek(0);
			if(!animator.isPlaying)
				animator.Play();
		}
	}
}
