using System;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhCard : MonoBehaviour
    {

        protected int CardValue;

        public void SetValue(int value)
        {
            UISprite spr = GetComponent<UISprite>();
            if (spr != null)
            {
                if (value != 100 && IsCardEffect(value))
                {
                    CardValue = value;
                    spr.spriteName = Convert.ToString(value, 16);
                }
                else
                {
                    spr.spriteName = Convert.ToString(100);
                }
            }
        }

        public void ShowFront()
        {
            if (IsCardEffect(CardValue))
            {
                UISprite spr = GetComponent<UISprite>();
                spr.spriteName = Convert.ToString(CardValue, 16);
                spr.color = Color.white;
            }
        }

        public void ShowBack()
        {
            UISprite spr = GetComponent<UISprite>();
            spr.spriteName = Convert.ToString(100);
            spr.color = Color.white;
        }

        public void ShowGray()
        {
            UISprite spr = GetComponent<UISprite>();
            spr.color = new Color(0x7B / 255.0f, 0x7B / 255.0f, 0x7B / 255.0f);
        }

        public void ShowLiangPai()
        {
            ShowFront();
            UISprite spr = GetComponent<UISprite>();
            spr.color = new Color(0x7B / 255.0f, 0x7B / 255.0f, 0x7B / 255.0f);
        }

        protected bool IsCardEffect(int value)
        {
            int color = value & 0xf0;
            int val = value & 0xf;
            if (color == 0x10 || color == 0x20 || color == 0x30 || color == 0x40)
            {
                if (val >= 2 && val <= 0xe)
                {
                    return true;
                }
            }

            if (value ==0x51|| value ==0x61)
            {
                return true;
            }

            return false;
        }
    }
}
