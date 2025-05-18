using Sfs2X.Entities.Data;
using Assets.Scripts.Game.ddz2.InheritCommon;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;
using System.Globalization;
using YxFramwork.Tool;
using System;
using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.TotalResultPanel
{
    public class ResultItem : MonoBehaviour
    {

        /// <summary>
        /// 玩家名字
        /// </summary>
        public UILabel UserName;
        /// <summary>
        /// 用户头像
        /// </summary>
        public YxBaseTextureAdapter UserIcon;
        /// <summary>
        /// 最高得分
        /// </summary>
        public UILabel BestScore;

        /// <summary>
        /// 总积分
        /// </summary>
        public UILabel AllScore;

        /// <summary>
        /// 胜利局数显示
        /// </summary>
        public UILabel WinLabel;
        /// <summary>
        /// 失败局数显示
        /// </summary>
        public UILabel LostLabel;
      
        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="overinfo">总结算给的用户信息</param>
        public void SetUserInfo(ISFSObject overinfo)
        {
            var gdata = App.GetGameData<DdzGameData>();
            var userSeat = overinfo.GetInt("seat");
            var userInfo = gdata.GetOnePlayerInfo(userSeat, true);
            int selfSeat = gdata.SelfSeat;
            if (userInfo == null) throw new Exception("GlobalData里没有相关座位用户信息");

            //名字
            UserName.text = userInfo.NickM;
            UserName.color = selfSeat == userSeat ? Color.yellow : Color.white;

            //头像
            PortraitDb.SetPortrait(userInfo.AvatarX, UserIcon, userInfo.SexI);

            //单局最高得分
            int maxScore = overinfo.GetInt("max");

            BestScore.text = YxUtiles.GetShowNumber(maxScore).ToString(CultureInfo.InvariantCulture);

            //胜局数
            WinLabel.text = string.Format("{0}胜",overinfo.GetInt("win"));

            //负局数
            LostLabel.text = string.Format("{0}负", overinfo.GetInt("lost"));


            //总积分
            int allScore = overinfo.GetInt("gold");
            AllScore.text = YxUtiles.GetShowNumber(allScore).ToString(CultureInfo.InvariantCulture);

        }
       
    }
}