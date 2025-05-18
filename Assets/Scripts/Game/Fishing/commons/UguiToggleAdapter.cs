using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

public class UguiToggleAdapter : YxBaseToggleAdapter {

    public Text DownLabel;
    public Text UpLabel;
    public override YxEUIType UIType
    {
        get { return YxEUIType.Ugui; }
    }

    private Toggle _toggle;
    public Toggle Toggle
    {
        get { return _toggle == null ? _toggle = GetComponent<Toggle>() : _toggle; }
    }

    public override bool SetSkinName(string skinName)
    {
        //todo 
        return false;
    }

    public override void SetLabel(string content)
    {
        if (DownLabel != null)
        {
            DownLabel.text = content;
        }
        if (UpLabel != null)
        {
            UpLabel.text = content; ;
        }
    }

    public override bool StartsActive {
        get
        {
            var toggle = Toggle;
            return toggle != null && toggle.isOn;
        }
        set
        {
            var toggle = Toggle;
            if (toggle != null)
            {
                toggle.isOn = value;
            }
        }
    }

    public override bool Value
    {
        get
        {
            var toggle = Toggle;
            return toggle != null && toggle.isOn;
        }
        set
        {
            var toggle = Toggle;
            if (toggle != null)
            {
                toggle.isOn = value;
            }
        }
    }

    public void OnChange(GameObject go)
    {
        go.SetActive(Value);
    }

    public void OnInverseChange(GameObject go)
    {
        go.SetActive(!Value);
    }
}
