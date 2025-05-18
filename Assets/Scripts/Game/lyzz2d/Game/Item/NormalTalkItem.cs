using System;
using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.Item
{
    public class NormalTalkItem : MonoBehaviour
    {
        public static Action OnItemClick;
        private int clickNum;

        private void Awake()
        {
            clickNum = int.Parse(name);
        }

        private void OnClick()
        {
            App.GetRServer<Lyzz2DGameServer>().SendUserTalk("#common:" + name);
            OnItemClick();
        }
    }
}