using System;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.View;
using Random = System.Random;
using System.Collections.Generic;
using YxFramwork.Framework;
using Assets.Scripts.Common.components;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.brtbsone
{
    public class BetCtrl : YxView
    {

        public GameObject Select;
        public GameObject ChipParents;
        public GridBetByCoin BetByCoin;
        public Chip CurrentSelectChip;
        public ChipConfig ChipCfg = new ChipConfig();
        public ChipConfig BankerChipCfg = new ChipConfig();
        public TweenInfo BetTweenInfo;
        public TweenInfo ResultTweenInfo;
        public Color[] SelectedBgColors;
        public int PlayerPoolsNum = 3;//因为推筒子可以是三个下注区域也可以是6个下注区域所有做成配置值
        public int BankerPoolMaxNum;
        public int PlayerPoolMaxNum;

        public Vector3 ChipScealVector3 = new Vector3(0.6f, 0.6f, 0.6f);

        protected int Chipdepth = 1;
        protected Transform ChipArea;
        private readonly Random _ran = new Random();
        protected int MaxGroupBet;
        private int _selfMaxBet;

        protected bool _isAllowBet = true;
        private long _lastClickTime;

        protected Pool<Chip>[] PlayerBetPools;
        protected Pool<Chip> BankerPool;

        protected override void OnStart()
        {
            Select.SetActive(false);
            Init();
        }

        public override void Init()
        {
            Chipdepth = 1;
            InitChipArea();
            if (BankerPool == null)
            {
                BankerPool = new Pool<Chip>(BankerPoolMaxNum, ChipCfg.ChipPerfab, transform);
            }
            if (PlayerBetPools == null || PlayerBetPools.Length < PlayerPoolsNum)
            {
                PlayerBetPools = new Pool<Chip>[PlayerPoolsNum];
                for (int i = 0; i < PlayerBetPools.Length; i++)
                {
                    PlayerBetPools[i] = new Pool<Chip>(PlayerPoolMaxNum, ChipCfg.ChipPerfab, transform);
                }
            }
        }

        public void InitChips()
        {
            var gdata = App.GetGameData<BrttzGameData>();
            OnBetClick(BetByCoin.InitChips(gdata.Ante, gdata.AnteRate));
        }

        /// <summary>
        /// 选择筹码
        /// </summary>
        /// <param name="chip"></param>
        public void OnBetClick(Chip chip)
        {
            int id = chip.GetData<ChipData>().BgId % SelectedBgColors.Length;
            Select.GetComponent<UISprite>().color = SelectedBgColors[id];
            var v = chip.transform.position;
            v = BetByCoin.transform.InverseTransformPoint(v);
            Select.transform.localPosition = new Vector3(v.x, v.y + 5, v.z);
            CurrentSelectChip = chip;
            Select.SetActive(true);
        }

        private void InitChipArea()
        {
            if (ChipArea != null)
            {
                //Destroy(ChipArea.gameObject);
                BankerPool.HideAllPrefab();
                foreach (var player in PlayerBetPools)
                {
                    player.HideAllPrefab();
                }
            }
            var temp = new GameObject("ChipArea");
            ChipArea = temp.transform;
            ChipArea.parent = ChipParents.transform;
            ChipArea.localPosition = Vector3.zero;
            ChipArea.localScale = Vector3.one;
            ChipArea.localRotation = Quaternion.identity;
        }

        public void CreateBetOnStart(int[] target, int rate)
        {
            for (int i = 0; i < target.Length; i++)
            {
                var temp = target[i] / rate;

            }
        }

        public virtual void GroupBet(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;
            var sfsArray = responseData.GetSFSArray("coin");
            var startPos = ChipCfg.StartPos;
            Facade.Instance<MusicManager>().Play("groupbet");
            var gdata = App.GetGameData<BrttzGameData>();
            var selfSeat = gdata.GetPlayerInfo().Seat;

            int maxGold = 0;
            // ReSharper disable once TooWideLocalVariableScope
            foreach (ISFSObject item in sfsArray)
            {
                string p = item.GetUtfString("p");
                int target = GetInt(p);
                int gold = item.GetInt("gold");
                int seat = item.GetInt("seat");
                if (seat == selfSeat)
                    continue;

                if (gold >= maxGold)
                {
                    maxGold = gold;
                }
                OtherMenBet(target, gold);
            }

            PlayAmazedSoud(maxGold, ref MaxGroupBet);
        }

        protected virtual void InstantiateChip(Transform ts, Vector3 localPos, int gold, bool needAnimo = true)
        {
            var chip = Instantiate(ChipCfg.ChipPerfab);
            var chipTs = chip.transform;
            chipTs.parent = ts.parent;
            chipTs.localPosition = localPos;
            chipTs.localScale = ChipScealVector3;
            chipTs.parent = ts;
            Chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = App.GetGameData<BrttzGameData>().AnteRate.IndexOf(gold),
                Depth = Chipdepth
            };
            chip.UpdateView(data);
            var label = chipTs.GetComponentInChildren<UILabel>();
            if (label != null)
                label.color = BetByCoin.LabelColors[data.BgId];
            chip.gameObject.SetActive(true);
            if (!needAnimo) { return; }
            Vector3 target = GetClipPos(ts);
            SetChipTween(chipTs, target, BetTweenInfo);
        }

        protected Chip GetOneChip(int target, bool isBanker = false)
        {
            var chip = isBanker ? BankerPool.GetOne() : PlayerBetPools[target].GetOne();
            return chip;
        }

        public virtual void InstantiateChip(int target, Vector3 localPos, int gold, bool isBanker = false, bool needAnimo = true)
        {
            //var chip = Instantiate(ChipCfg.ChipPerfab);
            var chip = GetOneChip(target, isBanker);
            var chipTs = chip.transform;
            var ts = ChipCfg.DeskAreas[target];
            chipTs.parent = ts.parent;
            chipTs.localPosition = localPos;
            chipTs.localScale = ChipScealVector3;
            chipTs.parent = ts;
            Chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = App.GetGameData<BrttzGameData>().AnteRate.IndexOf(gold),
                Depth = Chipdepth
            };
            chip.UpdateView(data);
            var label = chipTs.GetComponentInChildren<UILabel>();
            if (label != null)
                label.color = BetByCoin.LabelColors[data.BgId];
            chip.gameObject.SetActive(true);
            if (!needAnimo) { return; }
            Vector3 targetPos = GetClipPos(ts);
            SetChipTween(chipTs, targetPos, BetTweenInfo);
        }

        private void SetChipTween(Transform chipTs, Vector3 to, TweenInfo tweenInfo,
            List<EventDelegate> actionList = null)
        {
            var tp = chipTs.GetComponent<TweenPosition>();
            if (tp == null) return;

            Vector3 from = chipTs.localPosition;

            tp.from = from;
            tp.to = to;
            tp.animationCurve = tweenInfo.TweenCurve;
            tp.duration = tweenInfo.Duration;

            tp.onFinished = actionList == null ? tweenInfo.OnFinish : actionList;
            var chip = chipTs.GetComponent<BrttzChip>();
            if (chip == null) return;
            chip.DelayTime = tweenInfo.RandomDelay ? _ran.Next(0, 30) * 0.01f : tweenInfo.Delay;
            chip.BeginAnim();
        }

        public virtual void ThrowChips(int gold, int p, bool isBanker)
        {
            var startPos = isBanker ? BankerChipCfg.StartPos : ChipCfg.StartPos;
            var len = startPos.Length;
            var golds = GetGoldList(gold).ToArray();
            if (len > 0)
            {
                Facade.Instance<MusicManager>().Play("Bet");
                for (int i = 0; i < golds.Length; i++)
                {
                    //InstantiateChip(ChipCfg.DeskAreas[p], startPos[0], golds[i]);
                    InstantiateChip(p, startPos[0], golds[i], isBanker);
                }
            }
        }

        /// <summary>
        /// 移动每门下的筹码
        /// </summary>
        /// <param name="areaIndex">需要移动的区域</param>
        /// <param name="bankerWin">庄家是否赢了</param>
        public void MoveAllBet(int areaIndex, bool bankerWin)
        {
            var parent = ChipCfg.DeskAreas[areaIndex].transform;
            int childCount = parent.childCount;
            var startPos = bankerWin ? BankerChipCfg.StartPos : ChipCfg.StartPos;
            int len = startPos.Length;
            if (len > 0)
            {
                var target = startPos[0];
                //Transform item;
                for (int i = 0; i < childCount; i++)
                {
                    Transform item = parent.GetChild(0);
                    item.parent = ChipArea;
                    BrttzChip cs = item.GetComponent<BrttzChip>();
                    List<EventDelegate> de = new List<EventDelegate> { new EventDelegate(cs, "HideChip") };
                    SetChipTween(item.transform, target, ResultTweenInfo, de);
                }
            }
        }


        public virtual void OnDeskClick(Transform ts)
        {
            if (!_isAllowBet) { return; }
            var curClickTime = DateTime.Now.Ticks;
            if (curClickTime - _lastClickTime < 0.14f) { return; }
            _lastClickTime = curClickTime;
            if (CurrentSelectChip == null) { return; }
            var chipData = CurrentSelectChip.GetData<ChipData>();
            if (chipData == null) { return; }
            var gold = chipData.Value;
            if (CouldBet(gold))
            {
                var globadata = App.GetRServer<BrttzGameServer>();
                globadata.UserBet(ts.name, gold);
            }
        }

        public bool CouldBet(long gold)
        {
            var gdata = App.GetGameData<BrttzGameData>();
            if (!gdata.BeginBet)
            {
                YxMessageTip.Show("下注时间到，不能下注。");
                return false;
            }
            if (gold > gdata.CurrentCanInGold)
            {
                YxMessageTip.Show("庄家的金币不够，不能下注。");
                return false;
            }
            if (gold > gdata.ThisCanInGold)
            {
                YxMessageTip.Show("您的金币不够，不能下注。");
                return false;
            }
            if (gdata.BankerPlayer.Info.Seat == gdata.SelfSeat)
            {
                YxMessageTip.Show("您是庄家，不能下注。");
                return false;
            }
            return true;
        }

        public virtual void Bet(ISFSObject responseData)
        {
            var gold = responseData.GetInt(Parameter.Gold);
            var seat = responseData.GetInt(Parameter.Seat);
            var p = responseData.GetUtfString("p");
            int target = GetInt(p);
            var gdata = App.GetGameData<BrttzGameData>();
            if (seat == gdata.SelfSeat)
            {
                SelfMenBet(target, gold);
                gdata.ThisCanInGold = gdata.ThisCanInGold - gold;
                App.GetGameManager<BrttzGameManager>().CanQuitGame = false;
                return;
            }
            if (gold > gdata.AnteRate[gdata.AnteRate.Count - 1])
            {
                OtherMenBet(target, gold);
                return;
            }
            var startPos = ChipCfg.StartPos;
            var len = startPos.Length;
            if (len > 0)
            {
                //InstantiateChip(ChipCfg.DeskAreas[target], startPos[0], gold);
                InstantiateChip(target, startPos[0], gold);
                Facade.Instance<MusicManager>().Play("Bet");
            }
        }

        public int GetInt(string str)
        {
            var target = 0;
            switch (str)
            {
                case "s":
                    target = 0;
                    break;
                case "t":
                    target = 1;
                    break;
                case "x":
                    target = 2;
                    break;
                case "sj":
                    target = 3;
                    break;
                case "xj":
                    target = 4;
                    break;
                case "q":
                    target = 5;
                    break;
            }
            return target;
        }

        public virtual void SelfBet(int p, int gold)
        {
            var gdata = App.GetGameData<BrttzGameData>();
            gdata.SetGameStatus(YxEGameStatus.PlayAndConfine);
            gdata.GetPlayer().Coin -= gold;
            var lpos = GetChipPos(gold);
            lpos = ChipArea.InverseTransformPoint(lpos);
            //InstantiateChip(ChipCfg.DeskAreas[p], lpos, gold);
            InstantiateChip(p, lpos, gold);
        }

        protected void OtherBet(int p, int gold)
        {
            var startPos = ChipCfg.StartPos;
            //InstantiateChip(ChipCfg.DeskAreas[p], startPos[0], gold);
            InstantiateChip(p, startPos[0], gold);
        }

        protected void SelfMenBet(int p, int allGold)
        {
            //处理筹码值不是正好是配置筹码的情况
            var goldList = GetGoldList(allGold);
            foreach (var item in goldList)
            {
                SelfBet(p, item);
            }
        }

        protected void OtherMenBet(int p, int allGold)
        {
            var goldList = GetGoldList(allGold);
            foreach (var item in goldList)
            {
                OtherBet(p, item);
            }
        }

        protected Vector3 GetChipPos(int gold)
        {
            var gdata = App.GetGameData<BrttzGameData>();
            var anteRate = gdata.AnteRate;
            for (int i = 0; i < anteRate.Count; i++)
            {
                if (anteRate[i] == gold)
                {
                    var temp = i / BetByCoin.ChipNum == 0 ? i : i - BetByCoin.ChipNum;
                    int num = i / BetByCoin.ChipNum;
                    return BetByCoin.Grid[num].transform.GetChild(temp).position;
                }
            }
            return BetByCoin.Grid[0].transform.GetChild(0).position;
        }

        public List<int> GetGoldList(int gold)
        {
            var goldList = new List<int>();
            var anteRate = new List<int>(App.GetGameData<BrttzGameData>().AnteRate);
            anteRate.Sort((a, b) => a - b);
            int fristIndex = anteRate.Count - 1;

            for (int i = fristIndex; i >= 0; i--)
            {
                var val = anteRate[i];
                if (gold >= val)
                {
                    gold -= val;
                    goldList.Add(val);
                    i++;
                }
            }

            return goldList;
        }

        private int randonNum;

        public Vector3 GetClipPos(Transform ts)
        {
            var v = Vector3.zero;
            var r = new Random(RandonNumOnTime());
            var w = (int)ChipCfg.ChipAreas.x / 2;
            var h = (int)ChipCfg.ChipAreas.y / 2;
            var i2 = r.Next(-w, w);
            var i3 = r.Next(-h, h);
            v.x = i2;
            v.y = i3;
            return v;
        }

        private int RandonNumOnTime()
        {
            randonNum++;
            randonNum = randonNum > 1000 ? 0 : randonNum;
            return randonNum;
        }

        protected void PlayAmazedSoud(int gold, ref int maxBet)
        {
            var anteRate = App.GameData.AnteRate;

            if (gold > maxBet || anteRate[anteRate.Count - 1] == gold)
            {
                maxBet = gold;
                Facade.Instance<MusicManager>().Play("amazed");
            }
        }

        public virtual void HideChip()
        {
            BetByCoin.SetChipBtnsState(false);
            Select.SetActive(false);
        }

        public virtual void ShowChip()
        {
            var gdata = App.GetGameData<BrttzGameData>();
            if (gdata.IsBanker)
            {
                HideChip();
                return;
            }
            if (!gdata.BeginBet) return;
            BetByCoin.SetChipBtnsState(true);
            Select.SetActive(true);
        }

        public virtual void Reset()
        {
            var ante = App.GetGameData<BrttzGameData>().AnteRate;
            MaxGroupBet = ante[ante.Count - 1];
            _selfMaxBet = MaxGroupBet;
            ShowChip();
        }

        protected void OnDrawGizmos()
        {
            var arr = ChipCfg.StartPos;
            if (arr == null) return;
            var len = arr.Length;
            for (var i = 0; i < len; i++)
            {
                Gizmos.DrawSphere(transform.TransformPoint(ChipCfg.StartPos[i]), 0.1f);
            }
        }
    }
    [Serializable]
    public class ChipConfig
    {
        /// <summary>
        /// 筹码预设体
        /// </summary>
        public Chip ChipPerfab;
        public Vector3[] StartPos = new Vector3[0];
        public Transform[] DeskAreas;
        public Vector2 ChipAreas = new Vector2(100, 100);
    }
    [Serializable]
    public class TweenInfo
    {
        public AnimationCurve TweenCurve;
        public float Delay;
        public float Duration;
        public List<EventDelegate> OnFinish;
        [Tooltip("是否是随机延迟")]
        public bool RandomDelay;
    }

}
