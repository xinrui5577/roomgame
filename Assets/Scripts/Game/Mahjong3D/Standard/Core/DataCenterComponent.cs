using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class DataCenterComponent : BaseComponent, IGameReadyCycle, IGameStartCycle, IGameEndCycle, IContinueGameCycle
    {
        //游戏结束
        public bool IsGameOver { get; set; }
        //重连
        public bool IsReconect { get; set; }
        //解散房间状态
        public bool DissolvedState { get; set; }
        // 操作列表         
        public int OperateMenu { get; set; }
        // 第一次庄家位子 随机庄 计算圈用
        public int FristBankerSeat { get; set; }
        // 重连时手中牌状态[在手中:0],[已经打出:1]         
        public int ReconectCardState { get; set; }
        //（有听， 自己杠） 指定可杠得牌
        //（防止手把一时，杠牌换听）
        public List<int> GangCard { get; set; }

        /// <summary>
        /// 游戏全局数据缓存
        /// </summary>
        private Dictionary<RuntimeDataType, IRuntimeData> mDataMap = new Dictionary<RuntimeDataType, IRuntimeData>();

        public MahjongLocalConfig Config { get { return ConfigData.LocalConfig; } }
        public MahjongRoomData Room { get { return GetData<MahjongRoomData>(RuntimeDataType.Room); } }
        public MahjongSceneData Game { get { return GetData<MahjongSceneData>(RuntimeDataType.Game); } }
        public MahjongPlayersData Players { get { return GetData<MahjongPlayersData>(RuntimeDataType.Players); } }
        public MahjongConfigData ConfigData { get { return GetData<MahjongConfigData>(RuntimeDataType.Config); } }

        //庄家server座位号
        public int BankerSeat
        {
            get { return Game.BankerSeat; }
            set
            {
                Game.BankerSeat = value;
                GameCenter.EventHandle.Dispatch((int)EventKeys.SetBanker);
                var player = Players[0];
                if (player != null)
                {
                    GameCenter.Scene.OnSetBanker(value, player.Seat);
                }
            }
        }

        //庄家client座位号
        public int BankerChair
        {
            get { return MahjongUtility.GetChair(BankerSeat); }
        }

        //剩余麻将数
        public int LeaveMahjongCnt
        {
            get { return Game.LeaveMahjongCnt; }
            set
            {
                Game.LeaveMahjongCnt = value;
                GameCenter.EventHandle.Dispatch((int)EventKeys.UpdateMahCount);
            }
        }

        //当前操作玩家的server座位号
        public int CurrOpSeat
        {
            get { return Game.CurrOpSeat; }
            set
            {
                Game.OldOpSeat = Game.CurrOpSeat;
                Game.CurrOpSeat = value;
                GameCenter.Scene.ChangeDirection(Game.CurrOpSeat, Game.OldOpSeat);
                GameCenter.EventHandle.Dispatch((int)EventKeys.UpdateCurrOpPlayer);
            }
        }

        //上一个操作玩家的server座位号
        public int OldOpSeat
        {
            get { return Game.OldOpSeat; }
            set { Game.OldOpSeat = value; }
        }

        //当前操作玩家的client座位号
        public int CurrOpChair
        {
            get { return MahjongUtility.GetChair(CurrOpSeat); }
        }

        //上一个操作玩家的client座位号
        public int OldOpChair
        {
            get
            {
                if (OldOpSeat != DefaultUtils.DefValue) { return MahjongUtility.GetChair(OldOpSeat); }
                return DefaultUtils.DefValue;
            }
        }

        //骰子点数
        public int[] SaiziPoint
        {
            get { return Game.SaiziPoint; }
            set
            {
                Game.SaiziPoint = value;
                GameCenter.Scene.OnSetSaiziPoint(Game.SaiziPoint);
            }
        }

        /// <summary>
        /// 自己打牌标志
        /// </summary>
        public bool OwnerThrowoutCardFlag { get; set; }

        //打出的牌
        public int ThrowoutCard
        {
            get { return Game.ThrowoutCard; }
            set
            {
                Game.ThrowoutCard = value;
                var palyer = Players[CurrOpChair];
                if (null == palyer) return;
                palyer.OutCards.Add(value);
                if (CurrOpSeat == OneselfData.Seat)
                {
                    Game.IsOutPutCard = true;
                    Players[CurrOpChair].HardCards.Remove(value);
                    Players[CurrOpChair].OutCards.Add(value);
                }
                else
                {
                    if (Players[CurrOpChair].HardCards.Count > 0)
                    {
                        Players[CurrOpChair].HardCards.RemoveAt(0);
                    }
                }
                //麻将记录
                if (CurrOpChair != 0)
                {
                    GameCenter.Shortcuts.MahjongQuery.AddRecordMahjong(value);
                }
            }
        }

        //抓的牌
        public int GetInMahjong
        {
            get { return Game.GetInMahjong == 0 ? 0 : Game.GetInMahjong; }
            set
            {
                Game.GetInMahjong = value;
                var player = Players[CurrOpChair];
                if (null != player)
                {
                    player.HardCards.Add(value);
                }
                //麻将记录
                if (CurrOpChair == 0)
                {
                    GameCenter.Shortcuts.MahjongQuery.AddRecordMahjong(value);
                }
            }
        }

        //当前操作是自己
        public bool SelfCurrOp
        {
            get { return Players[0].Seat == CurrOpSeat; }
        }

        //自己服务器位置
        public int SelfSeat
        {
            get { return Players[0].Seat; }
        }

        //自动打牌：ting状态，并且没有Op操作时，或者托管时
        public bool AutoThrowoutCard
        {
            get
            {
                bool flag = false;
                // 抓牌用户 听牌的 并且 当前无菜单提示 自动打牌
                if (true)
                {
                    flag = CurrOpChair == 0 && Players[0].IsAuto && OperateMenu == 0;
                }
                return flag;
            }
        }

        // 本局最大人数
        public int MaxPlayerCount
        {
            get { return ConfigData.MaxPlayerCount; }
        }

        //自己信息
        public MahjongUserInfo OneselfData
        {
            get { return Players[0]; }
        }

        /// <summary>
        /// 房卡模式，游戏再进行中，第一局开始就是游戏中
        /// </summary>
        public bool IsGamePlaying
        {
            get
            {
                //lisi--因为局数位置问题修改--start--
                //原来:
                //return Room.RealityRound > 1 || GameCenter.GameProcess.IsCurrState<StateGamePlaying>();
                return Room.RealityRound > 0 || GameCenter.GameProcess.IsCurrState<StateGamePlaying>();
                //lisi--end--
            }
        }

        public T GetData<T>(RuntimeDataType type) where T : class, IRuntimeData
        {
            if (mDataMap.ContainsKey(type)) { return mDataMap[type] as T; }
            return null;
        }

        public bool IsLaizi(int card)
        {
            return card > 0 ? Game.LaiziCard == card : false;
        }

        public MahjongUserInfo CurrOpUserInfo()
        {
            return Players[CurrOpChair];
        }

        /// <summary>
        /// 设置游戏数据
        /// </summary>
        public void GetAllDatas(ISFSObject data)
        {
            //设置游戏数据
            mDataMap.ExIterationAction((item) => { item.Value.SetData(data); });
            //第一次庄家位子
            if (data.ContainsKey("bank0"))
            {
                FristBankerSeat = data.GetInt("bank0");
            }
            //重连
            IsReconect = data.TryGetBool(AnalysisKeys.KeyRejoin);
            if (IsReconect && GameCenter.Instance.IsGameStart())
            {
                CpgData cpg;
                int outCard = 0;
                //根据用户的麻将 算出剩余的麻将个数
                MahjongUserInfo userInfo;
                for (int i = 0; i < Players.CurrPlayerCount; i++)
                {
                    userInfo = Players[i];
                    if (!userInfo.ExIsNullOjbect())
                    {
                        outCard += userInfo.UserHardCardNum;
                        outCard += userInfo.OutCards.Count;
                        for (int j = 0; j < userInfo.CpgDatas.Count; j++)
                        {
                            cpg = userInfo.CpgDatas[j];
                            outCard += cpg.MahjongCount;
                        }
                        GameCenter.DataCenter.LeaveMahjongCnt = GameCenter.DataCenter.Room.MahjongCount - outCard;
                    }
                }
                int seq = data.GetInt(AnalysisKeys.KeySeq);
                int seq2 = data.GetInt(AnalysisKeys.KeySeq2);
                if (seq != seq2)//不相等 当前用户在抓牌
                {
                    OldOpSeat = (CurrOpSeat + MaxPlayerCount - 1) % MaxPlayerCount;
                    List<int> currOutCard = Players[OldOpChair].OutCards;
                    if (currOutCard.Count > 0 && currOutCard[currOutCard.Count - 1] != ThrowoutCard)
                    {
                        OldOpSeat = DefaultUtils.DefValue;
                    }
                    ReconectCardState = 0;
                }
                else//相等的时候 用户已经打牌 等待别人响应
                {
                    OldOpSeat = CurrOpSeat;
                    ReconectCardState = 1;
                }
            }
        }

        public void OnGameReadyCycle()
        {
            IsReconect = false;
            Players.ResetData();
        }

        public void OnGameStartCycle()
        {
            OwnerThrowoutCardFlag = false;
            Players.ResetReadyState();
            Players.BackupUserInfo();
        }

        public void OnGameEndCycle()
        {
            Game.IsOutPutCard = false;
            LeaveMahjongCnt = Room.MahjongCount;
            mDataMap.ExIterationAction((item) => { item.Value.ResetData(); });
        }

        public void OnContinueGameCycle()
        {
            Game.ClearTotalResult();
            DissolvedState = false;
            IsGameOver = false;
        }

        public override void OnInitalization()
        {
            GameCenter.RegisterCycle(this);
            GangCard = new List<int>();

            mDataMap.Add(RuntimeDataType.Players, new MahjongPlayersData());
            mDataMap.Add(RuntimeDataType.Config, new MahjongConfigData());
            mDataMap.Add(RuntimeDataType.Game, new MahjongSceneData());
            mDataMap.Add(RuntimeDataType.Room, new MahjongRoomData());
        }
    }
}