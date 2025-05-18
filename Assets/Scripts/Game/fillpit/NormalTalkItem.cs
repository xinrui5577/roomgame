using System;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.fillpit
{
    public class NormalTalkItem : MonoBehaviour
    {
        public static Action OnItemClick;

        protected void OnClick()
        {
            App.GetRServer<FillpitGameServer>().SendCommonText("#common:" +name);
            if(OnItemClick != null) OnItemClick();
        }
    }
}
