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

/*
 * 
 * This classs defines the control point
 * It is used to define the bezier curve
 * 
 */
[ExecuteInEditMode]
public class CameraPathBezierControlPoint : MonoBehaviour
{

    //Animation modes
    public enum animationEase
    {
        flat,//no change
        easein,
        easeout,
        easeinout
    }

    public enum DELAY_MODES
    {
        none,
        timed,
        indefinite
    }

    //the control point for this point - modifies the curve value for this end of the curve
    public Vector3 controlPoint = Vector3.zero;
    //a link ot the bezier curve class
    public CameraPathBezier bezier;
    //the animation mode to modify the curve time
    [SerializeField]
    private animationEase _ease = animationEase.flat;
    public AnimationCurve _curve;

    public float FOV = 60;

    public DELAY_MODES delayMode = DELAY_MODES.none;
    public float delayTime = 0;

    //variables to define the fustrum boxes displaed in the editor
    private float directionLineLength = 1.0f;
    private float focusBoxLength = 0.25f;
    [SerializeField]
    private float _tilt = 0;

    public bool useLongestRotation = false;

    [SerializeField]
    public Vector3 storedPosition = Vector3.zero;

    public Vector3 worldControlPoint
    {
        get
        {
            Vector3 returnPoint = bezier.transform.rotation * controlPoint;
            returnPoint += transform.position;
            return returnPoint;
        }
        set
        {
            //controlPoint = transform.InverseTransformPoint(value);
            Vector3 newValue = value - transform.position;
            newValue = Quaternion.Inverse(bezier.transform.rotation) * newValue;
            controlPoint = newValue;
        }
    }

    public Vector3 reverseWorldControlPoint
    {
        get
        {
            Vector3 returnPoint = bezier.transform.rotation * -controlPoint;
            returnPoint += transform.position;
            return returnPoint;
        }
        set
        {
            //controlPoint = transform.InverseTransformPoint(-value);
            Vector3 newValue = -value - transform.position;
            newValue = Quaternion.Inverse(bezier.transform.rotation) * newValue;
            controlPoint = newValue;
        }
    }

    public bool isLastPoint
    {
        get
        {
            if (bezier == null || bezier.controlPoints == null)
                return true;
            return this == bezier.controlPoints[bezier.numberOfControlPoints - 1];
        }
    }

    public AnimationCurve curve
    {
        get { return _curve; }
    }

    public animationEase ease
    {
        get
        {
            return _ease;
        }
        set
        {
            _ease = value;
            SetAnimationCurve();
        }
    }

    public float tilt
    {
        get
        {
            return _tilt;
        }
        set
        {
            _tilt = value;
            if (_tilt < -180)
                _tilt += 360;
            if (_tilt > 180)
                _tilt += -360;
        }
    }

    public Vector3 GetPathDirection()
    {
        float thisperc = bezier.GetPathPercentageAtPoint(this);
        float lastperc = Mathf.Clamp01(thisperc - 0.05f);
        float nextperc = Mathf.Clamp01(thisperc + 0.05f);
        Vector3 lastPos = bezier.GetPathPosition(lastperc);
        Vector3 nextPos = bezier.GetPathPosition(nextperc);
        return nextPos - lastPos;
    }

    public void SetRotationToCurve()
    {
        transform.LookAt(transform.position + GetPathDirection());
    }

    void OnEnable()
    {
        SetAnimationCurve();
    }

    void Update()
    {
        CheckPosition();
    }

    void OnDestroy()
    {
        if (bezier != null)
        {
            bezier.DeletePoint(this, false);
        }
    }

    void OnDrawGizmos()
    {

        Gizmos.DrawIcon(transform.position, "pathpoint");

        switch (bezier.mode)
        {
            case CameraPathBezier.viewmodes.usercontrolled:

                Vector3 directionTL = transform.TransformDirection(new Vector3(-focusBoxLength, -focusBoxLength, 1.0f) * directionLineLength) + transform.position;
                Vector3 directionTR = transform.TransformDirection(new Vector3(focusBoxLength, -focusBoxLength, 1.0f) * directionLineLength) + transform.position;
                Vector3 directionBL = transform.TransformDirection(new Vector3(-focusBoxLength, focusBoxLength, 1.0f) * directionLineLength) + transform.position;
                Vector3 directionBR = transform.TransformDirection(new Vector3(focusBoxLength, focusBoxLength, 1.0f) * directionLineLength) + transform.position;

                Gizmos.color = new Color(0, 0, 1, 0.6f);
                Gizmos.DrawLine(transform.position, directionTL);
                Gizmos.DrawLine(transform.position, directionTR);
                Gizmos.DrawLine(transform.position, directionBL);
                Gizmos.DrawLine(transform.position, directionBR);

                Gizmos.color = new Color(0, 0, 1, 0.4f);
                Gizmos.DrawLine(directionBL, directionTL);
                Gizmos.DrawLine(directionTL, directionTR);
                Gizmos.DrawLine(directionTR, directionBR);
                Gizmos.DrawLine(directionBR, directionBL);
                break;

            case CameraPathBezier.viewmodes.target:

                Transform target = bezier.target;
                if (target != null)
                {
                    Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.75f);
                    Gizmos.DrawRay(transform.position, (target.position - transform.position) * 0.9f);
                }
                break;

            //no need to show anything for the following
            case CameraPathBezier.viewmodes.mouselook:
                break;
            case CameraPathBezier.viewmodes.followpath:
                break;
            case CameraPathBezier.viewmodes.reverseFollowpath:
                break;
        }
    }

    private void SetAnimationCurve()
    {
        switch (_ease)
        {
            case animationEase.flat:
                _curve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
                break;
            case animationEase.easein:
                _curve = new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 1, 1));
                break;
            case animationEase.easeout:
                _curve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 0, 0));
                break;
            case animationEase.easeinout:
                _curve = new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 0, 0));
                break;
        }
    }

    //Ensure that the CP has not been moved
    public void CheckPosition()
    {
        if (transform.position != storedPosition)
        {
            bezier.RecalculateStoredValues();
            storedPosition = transform.position;
        }
    }
}
