using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.mx97
{
    public class ScoreJackpot : MonoBehaviour
    {
        public UISprite[] NumImgs;

        // Use this for initialization
        protected void Start()
        {
            Facade.EventCenter.AddEventListener<Mx97EventType, object>(Mx97EventType.RefreshJackpot, OnRefreshJackpot);
        }

        private void OnRefreshJackpot(object obj)
        {
            var gdata = App.GetGameData<Mx97GlobalData>();
            var imgLen = NumImgs.Length;
            var str = YxUtiles.GetShowNumber(gdata.Caichi).ToString().PadLeft(imgLen, '0');
            //Debug.Log(YxUtiles.GetShowNumber(gdata.Caichi));
            var numLen = str.Length;
            var index = numLen - 1;

            for (var i = imgLen - 1; i >= 0; i--)
                NumImgs[i].spriteName = "n_" + str[index--].ToString();
        }
    }
}