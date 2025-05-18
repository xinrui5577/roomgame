using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.BaiTuan
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

            switch (Status)
            {
                case 1:
                    App.GetRServer<BtwGameServer>().ApplyBanker();
                    break;
                case 0:
                    App.GetRServer<BtwGameServer>().ApplyQuit();
                    break;
                case 2:
                    var s = YxUtiles.ReduceNumber(App.GetGameData<BtwGameData>().MiniApplyBanker);
                    YxMessageTip.Show(string.Format("金币大于{0}可以申请成为团长...", s));
                    break;
            }
        }
        public virtual void ApplyOne()
        {

            switch (Status)
            {
                case 1:
                    App.GetRServer<BtwGameServer>().ApplyBankerOne();
                    break;
                case 0:
                    App.GetRServer<BtwGameServer>().ApplyQuitOne();
                    break;
                case 2:
                    var s = YxUtiles.ReduceNumber(App.GetGameData<BtwGameData>().MiniApplyBanker);
                    YxMessageTip.Show(string.Format("金币大于{0}可以申请成为团长...", s));
                    break;
            }
        }
    }
}
