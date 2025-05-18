using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Utils;
using com.yxixia.utile.YxDebug;
using YxFramwork.View;

namespace Assets.Scripts.Game.mx97
{
    public class Mx97GlobalData : YxGameData
    {
        public long Caichi = 0;			// 彩池金额
        public string Config = "";      // 倍数表显示的内容 把所有item拼接到一个整体sting "皇家同花顺:1000:80,同花顺:500:10,炸弹:100:5,葫芦:50:0,同花:20:0,顺子:15:0,三条:10:0,两对:5:0,一对:2:0,"


        public List<string> RadioList;
        public Dictionary<string, string> BootEnv;

        public int CurAnte { get; private set; }
        public int AnteRateAll { get { return CurAnte * 8; } }                       // 总的押注分数
        /// <summary>
        /// 当前的压注
        /// </summary>
        public int CurBet
        {
            get
            {
                return AnteRateAll * Ante;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            AnteRate.Clear();
            AnteRate.Add(1);
            AnteRate.Add(2);
            AnteRate.Add(4);
        }
         
        private int _curAnteIndex = -1;
        public void ChangeAnteRate()
        {
            var count = AnteRate.Count;
            _curAnteIndex++;
            if (_curAnteIndex >= count || _curAnteIndex < 0)
            {
                _curAnteIndex = 0;
            }
            CurAnte = AnteRate[_curAnteIndex];
        }

        private int _curScore ;                                                   // 当前分数 用户点击上分 每点击一次 减少总金币数 增加当前分数 步长为房间比率
        public int CurScore { get { return _curScore; } }
        public void AddCurScore()
        {
            int upScore = Ante*8;
            var selfPlayer = GetPlayer(SelfLocalSeat);
            if (selfPlayer.Coin <= upScore)
            {
                YxMessageTip.Show("您的金币不足！");
                return;
            }

            selfPlayer.Coin -= upScore;
            _curScore += upScore;
        }

        /// <summary>
        /// 减分
        /// </summary>
        public void DedCurScore()
        {
            if (_curScore <= 0)
            {
                YxMessageTip.Show("没有可下的分数啦！");
                return;
            }
            var selfPlayer = GetPlayer(SelfLocalSeat);
            selfPlayer.Coin += _curScore;
            _curScore = 0;
        }

        public void AddCurScore(int score)
        {
            _curScore = _curScore + score;
        } 

        private bool _mIsStopAutoStart = false;                                      // 是否停止自动开始 用于设置某些牌型出现后用户查看这些好牌
        public bool IsStopAutoStart { get { return _mIsStopAutoStart; } set { _mIsStopAutoStart = value; } }
        public void ChangeStopAutoStart()
        {

        }

        public bool IsEnableAudio { get { return PlayerPrefs.GetInt("setting_checkbox_audio", 1) == 1; } }

        private bool _mIsCardsForwardComplete = true;                                // 开始之后牌翻过来 只有全翻过来之后才可以开始下一局
        public bool IsCardsForwardComplete { get { return _mIsCardsForwardComplete; } set { _mIsCardsForwardComplete = value; } }

        private bool _mIsShowPrizeMaskComplete = false;                              // 中奖的遮罩层是否显示完成 只有完成才能开始下一局
        public bool IsShowPrizeMaskComplete { get { return _mIsShowPrizeMaskComplete; } set { _mIsShowPrizeMaskComplete = value; } }

        //private bool mIsCardsBackComplete = false;                                // 点击开始之后牌扣过去 扣过去完成之后才能发送开始请求
        //public bool IsCardsBackComplete { get { return mIsCardsBackComplete; } set { mIsCardsBackComplete = value; } }

        private readonly List<StructPrize> MPrizeList = new List<StructPrize>();      // 中彩名单
        public List<StructPrize> PrizeList { get { return MPrizeList; } }
        public void AddPrizeData(string prizeStr, bool needDisplay = true)
        {
            var prize = new StructPrize();
            prize.ParseDataFromString(prizeStr);
            prize.NeedDisplay = needDisplay;
            MPrizeList.Insert(0, prize);

            // 每次插入之后都判断长度
            if (20 < MPrizeList.Count)
                MPrizeList.RemoveAt(20);
        }

        private RespStart mStartData = new RespStart();                      // 点击开始按钮之后的回包数据
        public RespStart StartData { get { return mStartData; } }

        public void SetStartData(ISFSObject sfsObject)
        {
            mStartData.ParseData(sfsObject);
            AddCurScore(mStartData.MGotJackpotGlod);
            var player = GetPlayerInfo();
            player.CoinA = mStartData.MTotalGold;
        }
         
        public void SetGameData(ISFSObject sfsObject)                        // 保存游戏信息数据
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameInfo"></param>
        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            Ante = gameInfo.GetInt("ante");
            Caichi = gameInfo.GetLong("caichi");
            if (gameInfo.ContainsKey("jilu"))
            {
                var sfZhongJiangJiLu = gameInfo.GetUtfStringArray("jilu");
                foreach (var t in sfZhongJiangJiLu)
                {
                    // 保存启动时服务器发来的中奖纪录
                    AddPrizeData(t, false);
                }
                YxDebug.LogError("cuol");
            }
        }


        private readonly string[] _fruitName = { "YingTao", "ChengZi", "XiGua", "MangGuo", "LingDang", "ShuangQi", "BarHuang", "BarHong", "BarLan" };

        private readonly int[] _fruitValue = { 0, 1, 2, 3, 6, 7, 11, 12, 13 };

        // 获取一个随机的水果图片
        public string GetRanName()
        {
            var iRan = Random.Range(0, 8);
            return _fruitName[iRan];
        }

        // 根据ID获取图片名称
        public string GetNameById(int id)
        {
            for (int i = 0; i < _fruitName.Length && i < _fruitValue.Length; i++)
            {
                if (id != _fruitValue[i])
                    continue;

                return _fruitName[i];
            }

            Debug.Log("  ----> FruitData: Got fruit name faliure! \n");
            return GetRanName();
        }
    }
}

