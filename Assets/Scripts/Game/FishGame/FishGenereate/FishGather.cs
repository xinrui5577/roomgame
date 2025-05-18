using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Common.external.NemoFileIO;
using Assets.Scripts.Game.FishGame.Fishs;
using Assets.Scripts.Game.FishGame.Users;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.FishGenereate
{
    /// <summary>
    /// 鱼收集系统
    /// </summary>
    /// <remarks>
    /// 注意:
    /// 1.待收集鱼不能同一屏幕同时存在两条,要不计算增加赔率时会增加(玩家会亏).
    /// </remarks>
    public class FishGather : MonoBehaviour {
        //事件
        public delegate void Evt_PlayerGatherFish(Player player, Fish fish,int gatherIdx);
        public delegate void Evt_PayBonus(Player player,int bouns);

        //当 初始化玩家某条鱼已收集状态 时触发
        public Evt_PlayerGatherFish EvtPlayerGatherFishInited;

        //当 玩家收集到指定鱼 时触发
        public Evt_PlayerGatherFish EvtPlayerGatherFishActive;

        //当 玩家收集齐所有鱼 时触发,参数:fish为空,gatherIdx为-1
        public Evt_PlayerGatherFish EvtPlayerGatheredAllFish;
    
        //派发奖励时
        public Evt_PayBonus EvtPayBonus;
    
        //变量
        public Fish[] Prefab_GatherFishAry;//收集的鱼列表(与PersistentData<uint, uint> PlayerGatheredFishRecs相对应,最多32个)
        public int OddAdditivePerFish = 10;//每条额外奖励,不能设得太高,最好是感觉不出来的程度
        public float TimeDelayBonus = 4F;//奖励延时时间
        [System.NonSerialized]
        private PlayerBatterys PlayersBatterys;
        public PersistentData<uint, uint>[] PlayerGatheredFishRecs;//已收集鱼记录,结构(从右到左一个bit代表已收集到相应的鱼类型)
        public PersistentData<int, int>[] PlayerGatheredScore;//已收集分数

        public Dictionary<int, object>[] PlayerGatheredFish;//与PlayerGatheredFishRecs关联,key值是收集鱼列表的索引
        private Dictionary<int, int> mPrefabNeedGatherFishCache;//与Prefab_GatherFishAry关联,收集鱼列表, key:鱼typeIdx ,value:收集索引

        private bool mCanEditFishRecored = true;//是否可修改鱼记录状态(用于防止在同一帧中杀死多条鱼,而满足派彩条件并额外获得收集鱼的问题[玩家获益])
        /// <summary>
        /// 需要收集的数目
        /// </summary>
        public int CountFishNeedGather
        {
            get { return Prefab_GatherFishAry.Length; }
        }
        void Awake () {
            HitProcessor.AddFunc_Odd(Func_GetFishOddAddtiveForDieRatio,null); 
            PlayersBatterys = GameMain.Singleton.PlayersBatterys;
            mPrefabNeedGatherFishCache = new Dictionary<int, int>();
            for (int i = 0; i != Prefab_GatherFishAry.Length; ++i)
                mPrefabNeedGatherFishCache.Add(Prefab_GatherFishAry[i].TypeIndex, i);

            //创建PersistentData
            if (PlayerGatheredFishRecs == null)
            {
                PlayerGatheredFishRecs = new PersistentData<uint, uint>[Defines.MaxNumPlayer];
                for (int i = 0; i != Defines.MaxNumPlayer; ++i)
                    PlayerGatheredFishRecs[i] = new PersistentData<uint, uint>("PlayerGatheredFishRecs" + i.ToString());
            }
            if (PlayerGatheredScore == null)
            {
                PlayerGatheredScore = new PersistentData<int, int>[Defines.MaxNumPlayer];
                for (int i = 0; i != Defines.MaxNumPlayer; ++i)
                    PlayerGatheredScore[i] = new PersistentData<int, int>("PlayerGatheredScore" + i.ToString());
            }
            //创建 PlayerGatheredFish
            PlayerGatheredFish = new Dictionary<int, object>[Defines.MaxNumPlayer];
        

            //由PlayerGatheredFishRec得出字典PlayerGatheredFish
            for (int playerIdx = 0; playerIdx != Defines.MaxNumPlayer; ++playerIdx)
            {
                PlayerGatheredFish[playerIdx] = new Dictionary<int, object>();

                for (int gIdx = 0; gIdx != Prefab_GatherFishAry.Length; ++gIdx)//收集索引
                {
                    if (((PlayerGatheredFishRecs[playerIdx].Val >> gIdx) & 1) == 1)
                    {
                        PlayerGatheredFish[playerIdx].Add(gIdx,null);
                        //_tmpAddViewAniFish(gIdx, Players[playerIdx];
                    }
                }
            }


        }

        void Start()
        {
            //必须在start触发事件,如果再awake触发,其他类有可能响应不到
            for (int playerIdx = 0; playerIdx != Defines.MaxNumPlayer; ++playerIdx)
            {
                foreach (int gIdx in PlayerGatheredFish[playerIdx].Keys)
                {
                    if (EvtPlayerGatherFishInited != null)
                        EvtPlayerGatherFishInited(PlayersBatterys[playerIdx], Prefab_GatherFishAry[gIdx], gIdx);
                }
            }
        }
        /// <summary>
        /// 更新某玩家收集鱼的记录
        /// </summary>
        /// <param name="playerIdx"></param>
        /// <param name="gatherIdx">收集索引</param>
        /// <param name="isClear">是否清空收集记录</param>
        void UpdatePlayerGatheredFishRecs(int playerIdx,int gatherIdx,bool isClear)
        {
            if(PlayerGatheredFish[playerIdx] == null)
            {
                YxDebug.LogError(string.Format("[FishGather] UpdatePlayerGatheredFishRecs 更新失败.PlayerGatheredFish[{0}] 未初始化", playerIdx));
                return;
            }
            if(PlayerGatheredFishRecs[playerIdx] == null)
            {
                YxDebug.LogError(string.Format("[FishGather] UpdatePlayerGatheredFishRecs 更新失败.PlayerGatheredFishRecs[{0}] 未初始化", playerIdx));
                return;
            }

            if (isClear)
            {
                PlayerGatheredFish[playerIdx].Clear();
                PlayerGatheredFishRecs[playerIdx].Val = 0;
                return;
            }

            PlayerGatheredFish[playerIdx].Add(gatherIdx, null);

            uint recordNew = 0;
            foreach(KeyValuePair<int,object> kvp in PlayerGatheredFish[playerIdx])
            {
                recordNew |= ((uint)1 << kvp.Key);
            }

            PlayerGatheredFishRecs[playerIdx].Val = recordNew;
        }

        /// <summary>
        /// 响应用于计算鱼死亡额外倍率的函数
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        /// <param name="fCauser"></param>
        /// <returns></returns>
        /// <remarks>
        /// 注意:
        /// 1.当有全屏炸弹或者同类炸弹同时杀死两条同一TypeIdx的鱼时,他们赔率都会增倍,而正确做法应该是只有其中一条增加倍率.解决方案:待收集鱼同一屏幕不能同时存在两条
        /// </remarks>
        HitProcessor.OperatorOddFix Func_GetFishOddAddtiveForDieRatio(Player killer, Bullet b, Fish f, Fish fCauser)
        {
            //判断在不在需要收集列表中
            if (!mPrefabNeedGatherFishCache.ContainsKey(f.TypeIndex))
                return null;
            //判断在不在已收集列表中
            int gIdx = mPrefabNeedGatherFishCache[f.TypeIndex];//当前鱼的收集索引
            if (PlayerGatheredFish[killer.Idx].ContainsKey(gIdx))
                return null;
            //Debug.Log(string.Format("对鱼{0} 添加10难度",f.name));
            //在收集列表,且不在已收集列表 则添加额外倍率
            return new HitProcessor.OperatorOddFix(HitProcessor.Operator.Add,OddAdditivePerFish);
        }

        /// <summary>
        /// 响应鱼死亡事件
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        void Evt_FishKilled(Player killer, Bullet b, Fish f)
        {
            if (!mCanEditFishRecored)
                return;

            //判断在不在收集列表中
            if (!mPrefabNeedGatherFishCache.ContainsKey(f.TypeIndex))
                return;

            int gIdx = mPrefabNeedGatherFishCache[f.TypeIndex];//当前鱼的收集索引

            //判断在不在已收集列表中
            if (PlayerGatheredFish[killer.Idx].ContainsKey(gIdx))
                return;

            //判断是否收集到最后一个,如果是则需要清空,并放出奖金
            if (PlayerGatheredFish[killer.Idx].Count + 1 == Prefab_GatherFishAry.Length)
            {
                //放出奖金
                StartCoroutine(_Coro_DelayPayBonus(killer.Idx, PlayerGatheredScore[killer.Idx].Val + b.Score * OddAdditivePerFish));
            
                UpdatePlayerGatheredFishRecs(killer.Idx, 0, true);
                PlayerGatheredScore[killer.Idx].Val = 0;
                 
                //Debug.Log("最后一个条鱼,放出奖金,清空收集鱼记录");
                if (EvtPlayerGatherFishActive != null)
                    EvtPlayerGatherFishActive(PlayersBatterys[killer.Idx], Prefab_GatherFishAry[gIdx], gIdx);

                if (EvtPlayerGatheredAllFish != null)
                    EvtPlayerGatheredAllFish(PlayersBatterys[killer.Idx], null, -1);

                StartCoroutine(_Coro_CoolDownRecodeFish());
            }
            else
            {
                //在收集列表且不在已收集列表的,则改为已收集状态并记录分数
                UpdatePlayerGatheredFishRecs(killer.Idx, gIdx, false);
                PlayerGatheredScore[killer.Idx].Val += b.Score * OddAdditivePerFish;

                if (EvtPlayerGatherFishActive != null)
                    EvtPlayerGatherFishActive(PlayersBatterys[killer.Idx], Prefab_GatherFishAry[gIdx], gIdx);
            }
        }
        IEnumerator _Coro_CoolDownRecodeFish()
        {
            mCanEditFishRecored = false;
            yield return new WaitForSeconds(1F);
            mCanEditFishRecored = true;
        }
        /// <summary>
        /// 响应后台(两次打码)清空所有数据
        /// </summary>
        void Evt_BGClearAllData_Before()
        {
            for (int i = 0; i != Defines.MaxNumPlayer; ++i)
            {
                UpdatePlayerGatheredFishRecs(i, 0, true);
                PlayerGatheredScore[i].Val = 0;
            }
        }
         
        IEnumerator _Coro_DelayPayBonus(int playerIdx,int score)
        {
            yield return new WaitForSeconds(TimeDelayBonus);
            if (PlayersBatterys[playerIdx].Idx != playerIdx)
            {
                YxDebug.LogError("[FishGather] _Coro_DelayPayBonus,检查playerIdx出错.");
                yield break;
            }

            //Debug.Log(string.Format("玩家{0:d} 派发奖金 = {1:d}", playerIdx, score));
            PlayersBatterys[playerIdx].GainScore(score, 100, score / 100);

            if (EvtPayBonus != null)
                EvtPayBonus(PlayersBatterys[playerIdx], score);

        }
    }
}
