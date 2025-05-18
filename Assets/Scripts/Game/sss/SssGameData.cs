using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.sss
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class SssGameData : YxGameData
    {
       
        [HideInInspector]
        public bool SwatModel = true;

        /// <summary>
        /// 庄家座位号
        /// </summary>
        [HideInInspector]
        public int BankerSeat = 0;

        [HideInInspector]
        public bool IsBankerModel = false;

        /// <summary>
        /// 是否是房间模式
        /// </summary>
        [HideInInspector]
        public bool IsRoomGame = false;

        /// <summary>
        /// 当前局数,从1开始
        /// </summary>
        [HideInInspector]
        public int CurRound = 0;

        /// <summary>
        /// 游戏是否正在进行
        /// </summary>
        [HideInInspector]
        public bool IsPlaying = false;

        [HideInInspector]
        public int ChoiseModel;

        /// <summary>
        /// 游戏是否已经开始
        /// </summary>
        [HideInInspector]
        public bool IsPlayed
        {
            get { return CurRound > 0 || IsPlaying; }
        }

        /// <summary>
        /// 玩家开房模式
        /// 0.不消耗房卡 ; 1.房主消耗房卡 ; 2.所有人消耗房卡 ; 3.代开房间
        /// 此处之处理3模式,此变量用于判断是否是3号模式
        /// </summary>
        public int UserType = 3;

        /// <summary>
        /// 是否是代开房间
        /// </summary>
        public bool DaiKai { get; set; }

        /// <summary>
        /// 打枪的加分,为0时,为翻倍场
        /// </summary>
        [HideInInspector]
        public int ShootScore;

        /// <summary>
        /// 选择牌的时间
        /// </summary>
        [HideInInspector]
        public int PutTime;


        /// <summary>
        /// 用于将筹码修改为小数情况
        /// </summary>
        private int _showGoldRate = 1;

        public string RuleInfo = string.Empty;

        /// <summary>
        /// 用于将筹码修改为小数情况
        /// </summary>
        public int ShowGoldRate
        {
            set { _showGoldRate = value; }
            get
            {
                _showGoldRate = _showGoldRate > 0 ? _showGoldRate : 1;
                return _showGoldRate;
            }
        }
        /// <summary>
        /// 特殊牌型类型 过滤
        /// </summary>
        [HideInInspector]
        public List<int> SpecialTypes;

        public bool IsAllowSpecialType(int type)
        {
            return SpecialTypes == null || SpecialTypes.Contains(type);
        }

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new SssUserInfo();
            userInfo.Parse(userData);
            return userInfo;
        }

    }
}
