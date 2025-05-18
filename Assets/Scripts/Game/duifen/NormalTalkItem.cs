using System;
using Assets.Scripts.Game.duifen.ImgPress.Main;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.duifen
{
    public class NormalTalkItem : MonoBehaviour
    {
        public static Action OnItemClick;

        // ReSharper disable once UnusedMember.Local
        void OnClick()
        {
            App.GetRServer<DuifenGameServer>().SendCommonText("#common:" +name);
            if (OnItemClick == null)
                return;
            OnItemClick();
        }
    }
}
