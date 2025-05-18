using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketInputView : YxView
    {
        public NguiLabelAdapter ShowLabel;
        public NguiLabelAdapter ShowLabelSecond;
        public List<GameObject> Points;

        public void OnInput(string objName)
        {
            var str = ShowLabel.Value;
            if (str.Length >6)
            {
                ShowLabel.TrySetComponentValue(str.Substring(0, 6));
                return;
            }
            if (str.Equals("0.00"))
            {
                str = "";
            }
            if (objName.Equals("del"))
            {
                if (str.Length > 0)
                {
                    if (Points.Count != 0)
                    {
                        Points[str.Length - 1].SetActive(false);
                    }
                    str = str.Substring(0, str.Length - 1);
                }
            }
            else
            {
                if (str.Length != 6)
                {
                    str += objName;
                    if (Points.Count != 0)
                    {
                        Points[str.Length - 1].SetActive(true);
                    }
                }
            }
            ShowLabel.TrySetComponentValue(str);
            if (ShowLabelSecond)
            {
                ShowLabelSecond.TrySetComponentValue(str);
            }
        }
    }
}
