using System;
using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.Common.core;
using Sfs2X.Entities.Data;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.FishGame.Common.Utils 
{
    public class FishGameData :YxGameData
    {
        public Dictionary<string, object> UserInfor; 
        public ISFSObject GameInfo;
        public YxGameState GameState; 
        public long TotalCoin;//todo what 
        public int Msgrate;
        public int BaseUpperScore = 20;
        public bool NeedUpperScore = true;
        /// <summary>
        /// 可以购买分
        /// </summary>
        [NonSerialized]
        public bool CanBuyCoin = true;
        /// <summary>
        /// 可以下分
        /// </summary>
        [NonSerialized]
        public bool CanRetrieveCoin = true;
        #region 委托
        /// <summary>
        /// 停止出鱼
        /// </summary>
        public Event_Generic EvtStopGenerateFish;
        /// <summary>
        /// 开始游戏
        /// </summary>
        public Event_Generic EvtMainProcessStartGame;
        /// <summary>
        /// 准备过场.波浪过先
        /// </summary>
        public Event_Generic EvtMainProcessPrepareChangeScene;
        /// <summary>
        /// 过场完毕.波浪过先
        /// </summary>
        public Event_Generic EvtMainProcessFinishChangeScene;
        /// <summary>
        /// 鱼阵结束
        /// </summary>
        public Event_Generic EvtMainProcessFinishPrelude;
        /// <summary>
        /// 第一次进入场景 
        /// </summary>
        public Event_Generic EvtMainProcessFirstEnterScene;
        /// <summary>
        /// 
        /// </summary>
        public Event_Generic EvtFreezeBombActive;
        /// <summary>
        /// 
        /// </summary>
        public Event_Generic EvtFreezeBombDeactive;
        /// <summary>
        /// 鱼死亡事件
        /// </summary>
        public GameMain.EventFishKilled EvtFishKilled;
        /// <summary>
        /// 鱼炸弹爆炸,(注意:先触发.其他鱼再死亡)
        /// </summary>
        public GameMain.EventFishBombKilled EvtFishBombKilled;
        /// <summary>
        /// 同类炸弹爆炸
        /// </summary>
        public GameMain.EventFishBombKilled EvtSameTypeBombKiled;
        /// <summary>
        /// 超级同类炸弹爆炸
        /// </summary>
        public GameMain.EventFishBombKilled EvtSameTypeBombExKiled;
        /// <summary>
        /// 
        /// </summary>
        public GameMain.EventFishClear EvtFishClear;
        /// <summary>
        /// 
        /// </summary>
        public GameMain.EventFishClear EvtFishInstance;
        /// <summary>
        /// 领队初始化
        /// </summary>
        public GameMain.EventLeaderInstance EvtLeaderInstance;
        /// <summary>
        /// 
        /// </summary>
        public GameMain.EventPlayerGainScoreFromFish EvtPlayerGainScoreFromFish;
        /// <summary>
        /// 
        /// </summary>
        public GameMain.EventPlayerScoreChanged EvtPlayerScoreChanged;
        /// <summary>
        /// 
        /// </summary>
        public GameMain.EventPlayerGunChanged EvtPlayerGunChanged;
        /// <summary>
        /// 
        /// </summary>
        public GameMain.EventPlayerWonScoreChanged EvtPlayerWonScoreChanged; //玩家赢得分值改变('赢得分值'用于即时退币)
        /// <summary>
        /// 
        /// </summary>
        public GameMain.EventPlayerGunFired EvtPlayerGunFired; //开枪事件(发子弹)
        /// <summary>
        /// 
        /// </summary>
        public GameMain.EventBulletDestroy EvtBulletDestroy;
        /// <summary>
        /// 
        /// </summary>
        public GameMain.EventSoundVolumeChanged EvtSoundVolumeChanged;
        /// <summary>
        /// 
        /// </summary>
        public GameMain.EventKillLockingFish EvtKillLockingFish; //杀死正在锁定的鱼 
        /// <summary>
        /// 
        /// </summary>
        public GameMain.EventBackGroundClearAllDataBefore EvtBgClearAllDataBefore; //后台清空所有数据
        /// <summary>
        /// 后台改变场地类型
        /// </summary>
        public GameMain.EventBackGroundChangeArenaType EvtBgChangeArenaType;
        #endregion

        private readonly Dictionary<int, int> _fishCountInfo = new Dictionary<int, int>();

        public int FishCount(int fishId)
        {
            return _fishCountInfo.ContainsKey(fishId) ? _fishCountInfo[fishId] : 0;
        }

        public void RmoveFish(int fishId)
        {
            if (!_fishCountInfo.ContainsKey(fishId)) return; 
            var cur = _fishCountInfo[fishId] -= 1;
            if (cur >= 0) return;
            _fishCountInfo[fishId] = 0;
        }

        public void AddFish(int fishId)
        {
            if (_fishCountInfo.ContainsKey(fishId))
            {
                _fishCountInfo[fishId] += 1;
                return;
            }
            _fishCountInfo[fishId] = 1;
        }

        public bool RadiateMsg;
        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            RadiateMsg = gameInfo.ContainsKey("radiateMsg") && gameInfo.GetBool("radiateMsg");
        }

        public override void InitCfg(ISFSObject cargs2)
        {
            base.InitCfg(cargs2);
            var baseUp = "";
            if (SfsHelper.Parse(cargs2, "-baseUp", ref baseUp))//上分倍数
            {
                int upScore;
                if (int.TryParse(baseUp, out upScore))
                {
                    BaseUpperScore = upScore;
                }
            }
            var needUpStr = "";
            if (SfsHelper.Parse(cargs2, "-needUpCoin", ref needUpStr))
            {
                bool needUpCoin;
                if (bool.TryParse(needUpStr, out needUpCoin))
                {
                    NeedUpperScore = needUpCoin;
                }
            } 
        }

        public void ResetDepth()
        {
            _fishDepthDict.Clear();
        }

        private readonly Dictionary<float, float> _fishDepthDict = new Dictionary<float, float>();
        //获得鱼深度
        public float ApplyFishDepth(float fishDepth)
        {
            float curDepth;
            if (_fishDepthDict.ContainsKey(fishDepth))
            {
                curDepth = _fishDepthDict[fishDepth];
                curDepth += 0.00002f;
                _fishDepthDict[fishDepth] = curDepth;
            }
            else
            {
                curDepth = 0;
                _fishDepthDict[fishDepth] = curDepth;
            } 
            fishDepth += curDepth;
            return Defines.GMDepth_Fish - fishDepth; //  
        }
    }


    public enum YxGameState
    {
        Normal,
        Init,
        Run,
        Pause,
        Over
    }
}
