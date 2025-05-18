using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.slyz.Windows
{
    /// <summary>
    /// 倍数表
    /// </summary>
    public class AnteTableWindow : YxNguiWindow
    {
        public string SpecialDescribe = "{0}倍奖励\r\n另外奖励{1}% 奖池";
        public string NormalDescribe = "{0}倍";
        public AnteTableItemView[] AnteTableItemViews;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            //刷新奖励
            var gdata = App.GetGameData<SlyzGameData>();
            if (gdata == null) return;
            var config = gdata.Config;
            if (string.IsNullOrEmpty(config)) return;
            var infos = config.Split(',');
            var count = Mathf.Min(AnteTableItemViews.Length,infos.Length);
            
            for (var i = 0; i < count; i++)
            {
                var item = AnteTableItemViews[i];
                if (item == null) continue;
                var info = infos[i];
                var rowInfo = info.Split(':');
                var rowInfoCount = rowInfo.Length;
                if (rowInfoCount < 3) continue;
                var ante = rowInfo[1];
                var caichi = rowInfo[2];
                var msg = caichi == "0"
                    ? string.Format(NormalDescribe, ante)
                    : string.Format(SpecialDescribe, ante, caichi);
                item.SetNameLabe(rowInfo[0]);
                item.SetDescribeLabel(msg);
            }
        }
        //皇家同花顺:1000:80,同花顺:500:10,炸弹:100:5,葫芦:50:0,同花:20:0,顺子:15:0,三条:10:0,两对:5:0,对10以上:2:0,
    }
}
