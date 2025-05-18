using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.bjl3d
{
    public class SettleMentUI : MonoBehaviour// 改完11.14
    {
        public Text[] jiesuanTexts;
        public Text TotalText;
        public Text Xpoint;
        public Text Zpoint;

        /// <summary>
        /// 比赛结果
        /// </summary>
        public void GameResultFun()
        {
            Facade.Instance<MusicManager>().Play("JieSuan"); 
            var gdata = App.GetGameData<Bjl3DGameData>();
            var selfSeat = gdata.SelfSeat;
            for (var i = 0; i < gdata.BetJiesuan.Length; i++)
            {
                if (gdata.B == selfSeat)
                {
                    SetText(jiesuanTexts[i],gdata.BetMoney[i]);
                }
                else
                {
                    if (gdata.BetJiesuan[i] == 0)
                    {
                        SetText(jiesuanTexts[i], -gdata.BetMoney[i]);
                    }
                    else
                    {
                        SetText(jiesuanTexts[i], gdata.BetJiesuan[i] * gdata.BetMoney[i]);
                    }
                }
            }
            TotalText.text = YxUtiles.GetShowNumberToString(gdata.Win);
            Xpoint.text = gdata.XianValue + "";
            Zpoint.text = gdata.ZhuangValue + "";

        }


        protected void SetText(Text label,int coin)
        {
            label.text = YxUtiles.GetShowNumberToString(coin);
        }
    }
}