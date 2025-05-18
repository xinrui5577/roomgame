using System;
using Assets.Scripts.Game.sanpian.server;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sanpian.CommonCode
{
    public class PhizItem : MonoBehaviour
    {
        string  str;
        public static Action OnItemClick; 
        void Awake()
        {
            //clickNum = int.Parse(GetComponent<UISprite>().spriteName);
            str = GetComponent<UILabel>().text;
        }

        void OnClick()
        {
            App.GetRServer<SanPianGameServer>().SendUserTalk(str);
            OnItemClick();
        }
    }
}
