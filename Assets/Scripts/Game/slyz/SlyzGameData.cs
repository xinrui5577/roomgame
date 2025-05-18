using System.Collections.Generic;
using Assets.Scripts.Game.slyz.GameBase;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Framework.Core;
using YxFramwork.View;

//using CommonCode.ChessCommon;
//using Plats;
//using Assets.GameInHall;

namespace Assets.Scripts.Game.slyz
{
    public class SlyzGameData : YxGameData
    { 
        /// <summary>
        /// 彩池金额
        /// </summary>
        public int Caichi = 0;
        /// <summary>
        /// 倍数表显示的内容 把所有item拼接到一个整体sting "皇家同花顺:1000:80,同花顺:500:10,炸弹:100:5,葫芦:50:0,同花:20:0,顺子:15:0,三条:10:0,两对:5:0,一对:2:0,"
        /// </summary>
        public string Config = "";
        /// 服务器时间，秒
        public readonly GlobalMessage GMessage = new GlobalMessage();
        // 存储解析config后的list
        public List<Multible> MultibleList = new List<Multible>();
        private bool _isStopAutoStart = false;                                      // 是否停止自动开始 用于设置某些牌型出现后用户查看这些好牌
        public bool IsStopAutoStart { get { return _isStopAutoStart; } set { _isStopAutoStart = value; } }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            Ante = gameInfo.GetInt("ante");
            Caichi = gameInfo.GetInt("caichi");
            Config = gameInfo.GetUtfString("config");
      
            var gdata = App.GetGameData<SlyzGameData>();
            var sfZhongJiangJiLu = gameInfo.GetUtfStringArray("jilu");
            var len = sfZhongJiangJiLu.Length;
            for (var i = len - 1; 0 <= i; i--)
            {
                // 保存启动时服务器发来的中奖纪录
                gdata.AddPrizeData(sfZhongJiangJiLu[i], false);
            }

            var sfHaoPaiJiLu = gameInfo.GetSFSObject("user").GetUtfStringArray("goodCards");
            mStartData.ParseCardRecordInGameInfo(sfHaoPaiJiLu);
        }


        public void ChangeStopAutoStart()
        { 
            var iLen = mStartData.CardTeamList.Count;
            _isStopAutoStart = false;
            var curAutoStartState = AutoStartState;
            for (var i = 0; i < iLen; i++)
            {
                var type = mStartData.CardTeamList[i].type;
                if(type < curAutoStartState) continue;
                _isStopAutoStart = true;
                break;
            }
        }

        public int AutoStartState
        {
            get { return Util.GetInt(GetPlayerPrefsKey("setting_checkbox")); }
            set { Util.SetInt(GetPlayerPrefsKey("setting_checkbox"), value);}
        }

        public string GetPlayerPrefsKey(string key)
        {
            return string.Format("{0}_{1}", App.GameKey, key);
        }

        private bool _mIsCardsForwardComplete = true;                                // 开始之后牌翻过来 只有全翻过来之后才可以开始下一局
        public bool IsCardsForwardComplete { get { return _mIsCardsForwardComplete; } set { _mIsCardsForwardComplete = value; } }

        /// <summary>
        /// 
        /// </summary>
        public bool IsShowPrizeMaskComplete { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public readonly List<StructPrize> PrizeList =new List<StructPrize>();
        public void AddPrizeData(string prizeStr, bool needDisplay = true)
        {
            var prize = new StructPrize();
            prize.ParseDataFromString(prizeStr);
            prize.NeedDisplay = needDisplay;
            PrizeList.Insert(0, prize);
            Facade.EventCenter.DispatchEvent<ESlyzEventType,object>(ESlyzEventType.FreshPrizeList);
            // 每次插入之后都判断长度
            if (20 < PrizeList.Count)
            {
                PrizeList.RemoveAt(20);
            }
            if (!needDisplay) { return;}
            var noticeData = new YxNoticeMessageData { Message = prize.NoticeWords, ShowType = 1000 };
            YxNoticeMessage.ShowNoticeMsg(noticeData);
        }

        private RespStart mStartData = new RespStart();                      // 点击开始按钮之后的回包数据

        public SlyzGameData()
        {
            IsShowPrizeMaskComplete = false;
        }

        public RespStart StartData { get { return mStartData; } }
        public void SetStartData(ISFSObject sfsObject)
        {
            mStartData.ParseData(sfsObject);
        }
    }

    public class Multible
    {
        /// <summary>
        /// 皇家同花顺
        /// </summary>
        public string Name = "";
        /// <summary>
        /// 1000倍
        /// </summary>
        public string MultibleRate = "";
        /// <summary>
        /// 另外奖励80%奖池
        /// </summary>
        public string Caichi = "";              
    }

}
