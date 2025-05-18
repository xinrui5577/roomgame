using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.slyz.GameScene
{
    public class RightJackpot : YxView
    {
        /// <summary>
        /// 每注下注
        /// </summary>
        public YxBaseLabelAdapter AnteLabel;
        /// <summary>
        /// 奖池
        /// </summary>
        public YxBaseLabelAdapter CaiChiLabel;                                         // 动画的预设

        // Use this for initialization
        void Start () {

            var msg = App.GetGameData<SlyzGameData>().GMessage;
            msg.OnShowAllGoldInfo += OnShowAllGoldInfo;
            msg.OnShowTotalGlod += OnShowTotalGlod; 
        }

        public void FreshAnteLabel()
        {
            if (AnteLabel == null) return;
            var gdata = App.GetGameData<SlyzGameData>();
            AnteLabel.Text(gdata.Ante);
        }

        /// <summary>
        /// 上面代码优化部分
        /// </summary>
        private void OnShowAllGoldInfo()
        {
            var gdata = App.GetGameData<SlyzGameData>();
            FreshAnteLabel();
            FreshCaiChiLabel(gdata.Caichi);
        }

        private int _caichi;
        private void FreshCaiChiLabel(int caichi)
        {
            if (_caichi == caichi) return;
            _caichi = caichi;
            var value = StructPrize.GetShowNumberForm(_caichi, 0, "#");
            CaiChiLabel.Text(value);
        }


        private void OnShowTotalGlod()
        {
            var gdata = App.GetGameData<SlyzGameData>();
            var player = gdata.GetPlayer();
            player.Coin = player.Info.CoinA;
        }
    }
}
