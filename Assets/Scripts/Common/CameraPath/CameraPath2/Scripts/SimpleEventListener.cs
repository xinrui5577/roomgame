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

public class SimpleEventListener : MonoBehaviour
{
    [SerializeField]
    CameraPathBezierAnimator animator;

    void Awake()
    {

        animator.AnimationStarted += OnAnimationStarted;
        animator.AnimationPaused += OnAnimationPaused;
        animator.AnimationStopped += OnAnimationStopped;
        animator.AnimationFinished += OnAnimationFinished;
        animator.AnimationLooped += OnAnimationLooped;
        animator.AnimationPingPong += OnAnimationPingPonged;

        animator.AnimationPointReached += OnPointReached;
        animator.AnimationPointReachedWithNumber += OnPointReachedByNumber;

    }

    void Start()
    {
        animator.Play();
    }

    private void OnAnimationStarted()
    {
        Debug.Log("The animation has begun");
    }

    private void OnAnimationPaused()
    {
        Debug.Log("The animation has been paused");
    }

    private void OnAnimationStopped()
    {
        Debug.Log("The animation has been stopped");
    }

    private void OnAnimationFinished()
    {
        Debug.Log("The animation has finished");
    }

    private void OnAnimationLooped()
    {
        Debug.Log("The animation has looped back to the start");
    }

    private void OnAnimationPingPonged()
    {
        Debug.Log("The animation has ping ponged into the other direction");
    }

    private void OnPointReached()
    {
        Debug.Log("A point was reached");
    }

    private void OnPointReachedByNumber(int pointNumber)
    {
        Debug.Log("The point " + pointNumber + " was reached");
    }
}
