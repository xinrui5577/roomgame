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
using System.Collections.Generic;

/*
 * 
 * The Path class
 * This class deals with all things to do with building the path itself that your camera will follow
 * 
 */
public class CameraPathBezier : MonoBehaviour
{

    public enum viewmodes
    {
        usercontrolled,//rotations will be decided by the rotation of the contorl points
        target,//camera will always target a defined transform
        mouselook,//camera will have a mouse free look
        followpath,//camera will use the path to determine where to face - maintaining world up as up
        reverseFollowpath//camera will use the path to determine where to face, looking back on the path
    }

    public viewmodes mode = viewmodes.usercontrolled;
    //the transform to target in viewmode.target
    public Transform target;
    //the colour of the line in the Editor
    public Color lineColour = Color.white;
    //should the path loop
    public bool loop;

    //an array of control points that define the bezier curve
    [SerializeField]
    private CameraPathBezierControlPoint[] _controlPoints = new CameraPathBezierControlPoint[0];


    //this is the length of the arc of the entire bezier curve
    [SerializeField]
    private float _storedTotalArcLength = 0;
    //this is an arroy of arc lengths in a point by point basis
    [SerializeField]
    private float[] _storedArcLengths = null;
    //this is an array of arc lenths are intervals specified by storedArcLengthArraySize
    //it is the main data used in normalising the bezier curve to acheive a constant speed thoughout
    [SerializeField]
    private float[] _storedArcLengthsFull = null;
    //the amount of intervals in the _storedArcLengthsFull
    //you can modify this number to get a faster output for RecalculateStoredValues
    //750 seems to give a good accuracy
    //less = faster recalculation/lower accuracy
    //more = slower recalculation/higher accuracy
    [SerializeField]
    private int storedArcLengthArraySize = 750;

    private Quaternion lastRotation = Quaternion.identity;
    private int overRotate = 0;


    /////////////////
    //PUBLIC METHODS
    /////////////////

    //get the number of control points in the bezier curve
    public int numberOfControlPoints
    {
        get
        {
            if (controlPoints != null)
                return controlPoints.Length;
            else
                return 0;
        }
    }

    //get the number of curves in the bezier curve
    public int numberOfCurves
    {
        get
        {
            if (!loop)
                return _controlPoints.Length - 1;
            else
                return _controlPoints.Length;
        }
    }

    //get the array of control points
    public CameraPathBezierControlPoint[] controlPoints
    {
        get { return _controlPoints; }
    }

    //get the total arc length fo rthe entire curve
    public float storedTotalArcLength
    {
        get { return _storedTotalArcLength; }
    }

    //get the array of arc lengths - each arc length is between two points
    public float[] storedArcLengths
    {
        get { return _storedArcLengths; }
    }

    //calculate the stored values - for when the curve is modified or when we start
    //you shouldn't need to call this function but just in case you might want to call it - here it is
    //we're calculating the total arc length, individual arc lengths and a list of points specified by the storedArcLengthArraySize
    //the storedArcLengthFull is used to normalise the curve to attain a single speed thorughout the curve
    public void RecalculateStoredValues()
    {
        if (_controlPoints.Length < 2)
            return;
        float curveT;
        if (numberOfCurves < 1)
            curveT = 1.0f;
        else
            curveT = 1.0f / (float)numberOfCurves;

        _storedArcLengths = new float[numberOfCurves];
        for (int i = 0; i < numberOfCurves; i++)
            _storedArcLengths[i] = 0;
        float alTime = 1.0f / (storedArcLengthArraySize);
        float calculatedTotalArcLength = 0;
        _storedArcLengthsFull = new float[storedArcLengthArraySize];
        _storedArcLengthsFull[0] = 0.0f;
        for (int i = 0; i < storedArcLengthArraySize - 1; i++)
        {
            float altA = alTime * (i + 1);
            float altB = alTime * (i + 1) + alTime;
            Vector3 pA = GetPathPosition(altA);
            Vector3 pB = GetPathPosition(altB);
            float arcLength = Vector3.Distance(pA, pB);
            int arcpoint = Mathf.FloorToInt(altA / curveT);
            calculatedTotalArcLength += arcLength;
            _storedArcLengths[arcpoint] += arcLength;
            _storedArcLengthsFull[i + 1] = calculatedTotalArcLength;
        }
        _storedTotalArcLength = calculatedTotalArcLength;
    }

    public void Awake()
    {
        //Debug.Log("Control points "+controlPoints);
    }

    //This will delete all the data in the path.
    public void ResetPath()
    {
        while (_controlPoints.Length > 0)
            DeletePoint(0, true);
        RecalculateStoredValues();

    }

    //Delete a point in the path specified by the point itself.
    //deletePoint - component of the point
    //destroy - should the gameboject be destoryed?
    public void DeletePoint(CameraPathBezierControlPoint deletePoint, bool destroy)
    {
        List<CameraPathBezierControlPoint> rebuildList = new List<CameraPathBezierControlPoint>(_controlPoints);
        rebuildList.Remove(deletePoint);
        _controlPoints = rebuildList.ToArray();
        if (destroy)
            DestroyImmediate(deletePoint.gameObject);
        RecalculateStoredValues();
    }

    //Delete a point in the path specified by the point index.
    //index - cthe index in point array to delete
    //destroy - should the gameboject be destoryed?
    public void DeletePoint(int index, bool destroy)
    {
        List<CameraPathBezierControlPoint> rebuildList = new List<CameraPathBezierControlPoint>(_controlPoints);
        CameraPathBezierControlPoint deletePoint = rebuildList[index];
        rebuildList.RemoveAt(index);
        _controlPoints = new CameraPathBezierControlPoint[rebuildList.Count];
        _controlPoints = rebuildList.ToArray();
        if (destroy)
            DestroyImmediate(deletePoint.gameObject);
        RecalculateStoredValues();
    }

    //Add a new point at the end of the array
    public void AddNewPoint()
    {
        AddNewPoint(_controlPoints.Length);
    }

    //Add a new control point specified by the index of where you want to place it.
    public void AddNewPoint(int index)
    {
        GameObject newPoint = new GameObject("Control Point");
        newPoint.transform.parent = transform;

        if (numberOfControlPoints == 0)
        {
            newPoint.transform.localPosition = Vector3.zero;
        }
        else if (numberOfControlPoints == 1)
        {
            newPoint.transform.localPosition = _controlPoints[0].transform.rotation * Vector3.forward * 5.0f;
        }
        else
        {
            if (index < numberOfControlPoints)//place point within the path
            {
                CameraPathBezierControlPoint pointA = _controlPoints[Mathf.Clamp(index - 1, 0, numberOfUseablePoints)];
                CameraPathBezierControlPoint pointB = _controlPoints[Mathf.Clamp(index, 0, numberOfUseablePoints)];
                Vector3 p0 = pointA.transform.position;
                Vector3 p1 = pointA.worldControlPoint;
                Vector3 p2 = pointB.worldControlPoint;
                Vector3 p3 = pointB.transform.position;
                newPoint.transform.position = CalculateBezierPoint(0.5f, p0, p1, p2, p3);

                Quaternion p = _controlPoints[Mathf.Clamp(index - 2, 0, numberOfUseablePoints)].transform.rotation;
                Quaternion q = _controlPoints[Mathf.Clamp(index - 1, 0, numberOfUseablePoints)].transform.rotation;
                Quaternion a = _controlPoints[Mathf.Clamp(index, 0, numberOfUseablePoints)].transform.rotation;
                Quaternion b = _controlPoints[Mathf.Clamp(index + 1, 0, numberOfUseablePoints)].transform.rotation;
                newPoint.transform.rotation = CalculateCubicRotation(p, a, b, q, 0.5f);
            }
            else
            {//place point at the end of the path
                Vector3 endDirection = (GetPathPosition(1.0f) - GetPathPosition(0.95f)).normalized;
                newPoint.transform.position = _controlPoints[index - 1].transform.position + endDirection * 5.0f;
                newPoint.transform.rotation = _controlPoints[index - 1].transform.rotation;
            }
        }

        CameraPathBezierControlPoint cpScript = newPoint.AddComponent<CameraPathBezierControlPoint>();
        cpScript.name = "bezier control point " + index;
        cpScript.bezier = this;
        List<CameraPathBezierControlPoint> rebuildList = new List<CameraPathBezierControlPoint>(_controlPoints);
        rebuildList.Insert(index, cpScript);
        _controlPoints = rebuildList.ToArray();
        RecalculateStoredValues();
    }

    //Normalise the time based on the curve point
    //Put in a time and it returns a time that will account for arc lengths
    //Useful to ensure that path is animated at a constant speed
    //t - time(0-1)
    public float GetNormalisedT(float t)
    {
        if (t == 0)
            return 0;

        float targetLength = t * _storedTotalArcLength;

        int low = 0;
        int high = storedArcLengthArraySize;
        int index = 0;
        while (low < high)
        {
            index = low + ((high - low) / 2);
            if (_storedArcLengthsFull[index] < targetLength)
                low = index + 1;
            else
                high = index;
        }

        if (_storedArcLengthsFull[index] > targetLength && index > 0)
            index--;

        float lengthBefore = _storedArcLengthsFull[index];
        float currentT = (float)index / (float)storedArcLengthArraySize;
        if (lengthBefore == targetLength)
        {
            return currentT;
        }
        else
        {
            return (index + (targetLength - lengthBefore) / (_storedArcLengthsFull[index + 1] - lengthBefore)) / storedArcLengthArraySize;
        }
    }

    public int GetPointNumber(float t)
    {
        float curveT = 1.0f / (float)numberOfUseablePoints;
        return Mathf.Clamp(Mathf.FloorToInt(t / curveT), 0, (_controlPoints.Length - 1));
    }

    //Sample a position on the entire curve based on time (0-1)
    public Vector3 GetPathPosition(float t)
    {
        float curveT = 1.0f / (float)numberOfUseablePoints;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfUseablePoints);

        CameraPathBezierControlPoint pointA, pointB;
        pointA = GetPoint(point);
        pointB = GetPoint(point + 1);

        return CalculateBezierPoint(ct, pointA.transform.position, pointA.worldControlPoint, pointB.reverseWorldControlPoint, pointB.transform.position);
    }

    //Sample a rotation on the entire curve based on time (0-1)
    public Quaternion GetPathRotation(float t)
    {
        float curveT = 1.0f / (float)numberOfUseablePoints;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfUseablePoints);

        Quaternion p, q, a, b;
        p = GetPoint(point).transform.rotation;
        q = GetPoint(point + 1).transform.rotation;
        a = GetPoint(point - 1).transform.rotation;
        b = GetPoint(point + 2).transform.rotation;

        Quaternion ret = CalculateCubicRotation(p, a, b, q, ct);

        if (lastRotation != Quaternion.identity)
        {
            if (Quaternion.Angle(ret, lastRotation) > 90 && overRotate > 5)
            {
                ret = lastRotation;
                overRotate++;
            }
            else
            {
                overRotate = 0;
            }
        }

        lastRotation = ret;

        return ret;
    }

    //Sample the FOV on the entire curve based on time (0-1)
    public float GetPathFOV(float t)
    {
        float curveT = 1.0f / (float)numberOfUseablePoints;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfUseablePoints);

        float FOVA = GetPoint(point).FOV;
        float FOVB = GetPoint(point + 1).FOV;

        return Mathf.Lerp(FOVA, FOVB, CalculateHermite(ct));
    }

    //Sample the Tilt on the entire curve based on time (0-1) - only for followpath mode
    public float GetPathTilt(float t)
    {
        float curveT = 1.0f / (float)numberOfUseablePoints;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfUseablePoints);

        float tiltA = GetPoint(point).tilt;
        float tiltB = GetPoint(point + 1).tilt;

        return Mathf.Lerp(tiltA, tiltB, CalculateHermite(ct));
    }

    public float GetPathPercentageAtPoint(CameraPathBezierControlPoint point)
    {
        List<CameraPathBezierControlPoint> cps = new List<CameraPathBezierControlPoint>(_controlPoints);
        int index = cps.IndexOf(point);
        return ((float)index) / ((float)numberOfCurves);
    }

    /////////////////
    // MONOBEHAVIOURS
    /////////////////

    void OnDrawGizmos()
    {
        Gizmos.color = lineColour;
        if (numberOfUseablePoints < 1)
            return;

        for (float t = 0.0f; t <= 1.0f; t += 0.015f)
        {
            Gizmos.DrawLine(GetPathPosition(t), GetPathPosition(t + 0.013f));
        }
    }

    void OnEnable()
    {
        RecalculateStoredValues();
    }

    void Start()
    {
        RecalculateStoredValues();
    }

    /////////////////
    //PRIVATE METHODS
    /////////////////

    //returns a point from an specified index in the control point array
    //mainly here to deal with loops
    private CameraPathBezierControlPoint GetPoint(int index)
    {
        if (_controlPoints.Length == 0)
            return null;
        if (!loop)
        {
            return _controlPoints[Mathf.Clamp(index, 0, numberOfUseablePoints)];
        }
        else
        {
            if (index >= numberOfControlPoints)
                index = index - numberOfControlPoints;
            if (index < 0)
                index = index + numberOfControlPoints;

            return _controlPoints[index];
        }
    }

    //number of useable points - useful for curve based calculations
    private int numberOfUseablePoints
    {
        get
        {
            if (!loop)
                return _controlPoints.Length - 1;
            else
                return _controlPoints.Length;
        }
    }

    //Calculate the Bezier spline position
    //t - the time (0-1) of the curve to sample
    //p0 - the start point of the curve
    //p1 - control point from p0
    //p2 - control point from p3
    //p3 - the end point of the curve
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        float u = 1.0f - t;
        float u2 = u * u;
        float u3 = u2 * u;

        Vector3 p = u3 * p0 + 3 * u2 * t * p1 + 3 * u * t2 * p2 + t3 * p3;

        return p;
    }

    //Calculate Cubic Rotation
    //p - point we start with
    //q - next point
    //a - the point immediately before p
    //b - the point immediately after q
    //t - time (0-1) of the curve pq to sample
    private Quaternion CalculateCubicRotation(Quaternion p, Quaternion a, Quaternion b, Quaternion q, float t)
    {
        Quaternion a1 = SquadTangent(a, p, q);
        Quaternion b1 = SquadTangent(p, q, b);
        float slerpT = 2.0f * t * (1.0f - t);
        Quaternion sl = Slerp(Slerp(p, q, t), Slerp(a1, b1, t), slerpT);
        return sl;
    }

    private float CalculateHermite(float val)
    {
        return val * val * (3.0f - 2.0f * val);
    }

    /////////////////////////////////////
    //Warning - here be crazy maths stuff
    /////////////////////////////////////

    //calculate the Squad tangent for use in Cubic Rotation Interpolation
    private Quaternion SquadTangent(Quaternion before, Quaternion center, Quaternion after)
    {
        Quaternion l1 = lnDif(center, before);
        Quaternion l2 = lnDif(center, after);
        Quaternion e = Quaternion.identity;
        for (int i = 0; i < 4; ++i)
        {
            e[i] = -0.25f * (l1[i] + l2[i]);
        }
        return center * (exp(e));
    }

    private Quaternion lnDif(Quaternion a, Quaternion b)
    {
        Quaternion dif = Quaternion.Inverse(a) * b;
        Normalize(dif);
        return log(dif);
    }

    private Quaternion Normalize(Quaternion q)
    {
        float norm = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        if (norm > 0.0f)
        {
            q.x /= norm;
            q.y /= norm;
            q.z /= norm;
            q.w /= norm;
        }
        else
        {
            q.x = (float)0.0f;
            q.y = (float)0.0f;
            q.z = (float)0.0f;
            q.w = (float)1.0f;
        }
        return q;
    }

    private Quaternion exp(Quaternion q)
    {
        float theta = Mathf.Sqrt(q[0] * q[0] + q[1] * q[1] + q[2] * q[2]);

        if (theta < 1E-6)
        {
            return new Quaternion(q[0], q[1], q[2], Mathf.Cos(theta));
        }
        else
        {
            float coef = Mathf.Sin(theta) / theta;
            return new Quaternion(q[0] * coef, q[1] * coef, q[2] * coef, Mathf.Cos(theta));
        }
    }

    private Quaternion log(Quaternion q)
    {
        float len = Mathf.Sqrt(q[0] * q[0] + q[1] * q[1] + q[2] * q[2]);

        if (len < 1E-6)
        {
            return new Quaternion(q[0], q[1], q[2], 0.0f);
        }
        else
        {
            float coef = Mathf.Acos(q[3]) / len;
            return new Quaternion(q[0] * coef, q[1] * coef, q[2] * coef, 0.0f);
        }
    }

    //based on [Shoe87] implementation
    private Quaternion Slerp(Quaternion p, Quaternion q, float t)
    {
        Quaternion ret;
        float cos = Quaternion.Dot(p, q);

        float omega, somega, invSin, fCoeff0, fCoeff1;
        if ((1.0f + cos) > 0.00001f)
        {
            if ((1.0f - cos) > 0.00001f)
            {
                omega = Mathf.Acos(cos);
                somega = Mathf.Sin(omega);
                invSin = (Mathf.Sign(somega) * 1.0f) / somega;
                fCoeff0 = Mathf.Sin((1.0f - t) * omega) * invSin;
                fCoeff1 = Mathf.Sin(t * omega) * invSin;
            }
            else
            {
                fCoeff0 = 1.0f - t;
                fCoeff1 = t;
            }
            ret.x = fCoeff0 * p.x + fCoeff1 * q.x;
            ret.y = fCoeff0 * p.y + fCoeff1 * q.y;
            ret.z = fCoeff0 * p.z + fCoeff1 * q.z;
            ret.w = fCoeff0 * p.w + fCoeff1 * q.w;
        }
        else
        {
            fCoeff0 = Mathf.Sin((1.0f - t) * Mathf.PI * 0.5f);
            fCoeff1 = Mathf.Sin(t * Mathf.PI * 0.5f);

            ret.x = fCoeff0 * p.x - fCoeff1 * p.y;
            ret.y = fCoeff0 * p.y + fCoeff1 * p.x;
            ret.z = fCoeff0 * p.z - fCoeff1 * p.w;
            ret.w = p.z;
        }
        return ret;
    }

    private Quaternion Nlerp(Quaternion p, Quaternion q, float t)
    {
        Quaternion ret;

        float w1 = 1.0f - t;

        ret.x = w1 * p.x + t * q.x;
        ret.y = w1 * p.y + t * q.y;
        ret.z = w1 * p.z + t * q.z;
        ret.w = w1 * p.w + t * q.w;
        Normalize(ret);

        return ret;
    }

    private Quaternion GetQuatConjugate(Quaternion q)
    {
        return new Quaternion(-q.x, -q.y, -q.z, q.w);
    }
}
