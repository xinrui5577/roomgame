using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class TimeSign : MonoBehaviour
    {
        public Text TimeText;
        public void UpdataTimeText()
        {
            if (TimeText != null)
                TimeText.text = DateTime.Now.ToString("yyyy/MM/dd hh:mm");
        }
    }
}