using System;
using Assets.Scripts.Game.GangWu.Main;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.GangWu
{
    public class NormalTalkItem : MonoBehaviour
    {
        public static Action OnItemClick;

        protected void OnClick()
        {
            App.GetRServer<GangWuGameServer>().SendCommonText("#common:" + name);
            OnItemClick();
        }
    }
}
