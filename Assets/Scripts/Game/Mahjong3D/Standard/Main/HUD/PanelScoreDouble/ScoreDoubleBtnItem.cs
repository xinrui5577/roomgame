using UnityEngine.UI;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ScoreDoubleBtnItem : MonoBehaviour
    {
        public Text Number;
        public Action<int> OnClick;

        private int mScore;

        public void OnInit(string context, int score, int fontsize)
        {
            mScore = score;
            Number.text = context;
            Number.fontSize = fontsize;
        }

        public void OnJiapiaoClick()
        {
            if (null != OnClick) { OnClick(mScore); }
        }
    }
}