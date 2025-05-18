using System;
using Assets.Scripts.Game.sanpian.server;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sanpian.CommonCode
{
    public class NormalTalkItem : MonoBehaviour
    {
        public static Action OnItemClick;
        int clickNum;
        void Awake()
        {
            clickNum = int.Parse(name);
        }

        void OnClick()
        {
            App.GetRServer<SanPianGameServer>().SendUserTalk("#common:" +name);
            OnItemClick();
        }
    }
}
