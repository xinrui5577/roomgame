//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Simple example script of how a button can be scaled visibly when the mouse hovers over it or it gets pressed.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Scale")]
public class UIButtonScale : MonoBehaviour
{
	public Transform tweenTarget;
	public Vector3 hover = new Vector3(1.1f, 1.1f, 1.1f);
	public Vector3 pressed = new Vector3(1.05f, 1.05f, 1.05f);
	public float duration = 0.2f;
    public UIButton Button;
	Vector3 mScale;
	bool mStarted = false;

	void Start ()
	{
		if (!mStarted)
		{
			mStarted = true;
		    if (tweenTarget == null) tweenTarget = transform; 
			mScale = tweenTarget.localScale;
		}
	}

    public Transform GetTweenTarget()
    {
        if (Button == null)
        {
            return tweenTarget;
        }
        var btnTarget = Button.tweenTarget;
        return btnTarget == null? tweenTarget: btnTarget.transform; 
    }

    void OnEnable () { if (mStarted) OnHover(UICamera.IsHighlighted(gameObject)); }

	void OnDisable ()
	{
	    var twTarget = GetTweenTarget();
        if (mStarted && twTarget != null)
		{
			TweenScale tc = twTarget.GetComponent<TweenScale>();

			if (tc != null)
			{
				tc.value = mScale;
				tc.enabled = false;
			}
		}
	}

	void OnPress (bool isPressed)
	{
		if (enabled)
		{
		    var twTarget = GetTweenTarget();
            if (!mStarted) Start();
			TweenScale.Begin(twTarget.gameObject, duration, isPressed ? Vector3.Scale(mScale, pressed) :
				(UICamera.IsHighlighted(gameObject) ? Vector3.Scale(mScale, hover) : mScale)).method = UITweener.Method.EaseInOut;
		}
	}

	void OnHover (bool isOver)
	{
		if (enabled)
		{
		    var twTarget = GetTweenTarget();
            if (!mStarted) Start();
			TweenScale.Begin(twTarget.gameObject, duration, isOver ? Vector3.Scale(mScale, hover) : mScale).method = UITweener.Method.EaseInOut;
		}
	}

	void OnSelect (bool isSelected)
	{
		if (enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
			OnHover(isSelected);
	}
}
