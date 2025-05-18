using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.bjl3d
{
    public class Bjld3DPlayer : YxBaseGamePlayer// g 11.15
    {
        public Text UserAchievementText;
 
        public int TodayWin
        {
            get { return App.GetGameData<Bjl3DGameData>().TodayWin; }
            set
            {
                App.GetGameData<Bjl3DGameData>().TodayWin = value;
                SetTodayWin(value);
            }
        }

        /// <summary>
        /// 今天赢得次数
        /// </summary>
        /// <param name="todayWin"></param>
        protected virtual void SetTodayWin(int todayWin)
        {
            if (UserAchievementText == null) return;
            UserAchievementText.text = YxUtiles.GetShowNumberToString(todayWin);
        }

        protected override void FreshUserInfo()
        {
            base.FreshUserInfo();
            SetTodayWin(TodayWin);
        }
         
    }

}