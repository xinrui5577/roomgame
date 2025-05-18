using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.brnn
{
    public class ApplyCtrl : MonoBehaviour
    {
        public int Status = 1;
        public static ApplyCtrl Instance;

        [Tooltip("0:申请下庄；1:申请上庄；2:禁止上庄")]
        public GameObject[] StatusButtons;
        public void Start()
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

        /// <summary>
        /// 0:申请下庄 ; 1:申请上庄 ; 2:禁止上庄
        /// </summary>
        /// <param name="s"></param>
        public virtual void SetStutus(int s)
        {
            Status = s;
            if (StatusButtons == null) { return;}
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
                case 0:
                    App.GetRServer<BrnnGameServer>().ApplyQuit();
                    if (App.GetGameData<BrnnGameData>().IsBanker)
                    {
                        YxMessageTip.Show("此局游戏结束后自动下庄，请稍后！！！");
                    }
                    break;
                case 1:
                    App.GetRServer<BrnnGameServer>().ApplyBanker();
                    break;
                case 2:
                    YxMessageTip.Show("金币大于" + YxUtiles.ReduceNumber(App.GetGameData<BrnnGameData>().MiniApplyBanker) +
                                      "可以申请成为庄家...");
                    break;
            }
        }

    }


    public class BankerStatus
    {
        /// <summary>
        /// 可以上庄
        /// </summary>
        public const int CouldApply = 0;

        /// <summary>
        /// 可以下庄
        /// </summary>
        public const int CouldRelieve = 1;

        /// <summary>
        /// 无法上庄
        /// </summary>
        public const int CantApply = 2;

    }

}
