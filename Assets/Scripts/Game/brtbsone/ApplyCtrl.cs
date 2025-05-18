using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.brtbsone
{
    public class ApplyCtrl : MonoBehaviour
    {
        public int Status = 1;
        [Tooltip("0:申请下庄；1:申请上庄；2:禁止上庄")]
        public GameObject[] StatusButtons;

        public static ApplyCtrl Instance;

        protected void Start()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        public void RefreshBanker()
        {
            SetStutus(Status);
            gameObject.SetActive(true);
        }

        public void HideApplyBanker()
        {
            gameObject.SetActive(false);
        }

        public virtual void SetStutus(int s)
        {
            Status = s;
            if (StatusButtons == null) { return; }
            var count = StatusButtons.Length;
            for (var i = 0; i < count; i++)
            {
                var btn = StatusButtons[i];
                btn.SetActive(i == s);
            }
        }

        public virtual void Apply()
        {
            var gdata = App.GetGameData<BrttzGameData>();
            switch (Status)
            {
                case 1:
                    App.GetRServer<BrttzGameServer>().ApplyBanker();
                    break;
                case 0:
                    App.GetRServer<BrttzGameServer>().ApplyQuit();
                    if (gdata.GetPlayerInfo(0).Seat == gdata.BankerPlayer.Info.Seat)
                        YxMessageTip.Show("此局游戏结束后自动下庄，请稍后！！！");
                    break;
                case 2:
                    var s = YxUtiles.ReduceNumber(App.GetGameData<BrttzGameData>().MiniApplyBanker);
                    YxMessageTip.Show(string.Format("金币大于{0}可以申请成为团长...", s));
                    break;
            }
        }
    }
}