using UnityEngine;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class RankItem : MonoBehaviour
    {
        /// <summary>
        /// 排行的名次
        /// </summary>
        public UILabel UserRank;
        /// <summary>
        /// 玩家的名字
        /// </summary>
        public UILabel UserName;
        /// <summary>
        /// 需要显示的玩家的数据
        /// </summary>
        public UILabel UserWin;
        /// <summary>
        /// 排行榜的背景
        /// </summary>
        public GameObject RankBg;
        /// <summary>
        /// 设置排行榜中的数据
        /// </summary>
        /// <param name="userRank"></param>
        /// <param name="userName"></param>
        /// <param name="userWin"></param>
        public void SetData(string userRank,string userName,string userWin)
        {
            if (!userRank.Equals("未入榜")) RankBg.SetActive(true);
            UserRank.text = userRank;
            UserName.text = userName;
            UserWin.text = userWin;
        }
    }
}
