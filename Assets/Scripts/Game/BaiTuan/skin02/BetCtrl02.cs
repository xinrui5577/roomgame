using System;
using UnityEngine;
using YxFramwork.Common;
using Assets.Scripts.Common.components;
using System.Collections.Generic;
using Random = System.Random;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Enums;
using Sfs2X.Entities.Data;


namespace Assets.Scripts.Game.BaiTuan.skin02
{
    public class BetCtrl02 : BetCtrl
    {
        [SerializeField]
        public ChipConfig BankerChipCfg = new ChipConfig();
        private int _selfMaxBet;
        public TweenInfo BetTweenInfo;
        public TweenInfo ResultTweenInfo;

        private readonly Random _ran = new Random();



        protected override void InstantiateChip(Transform ts, Vector3 localPos, int gold, bool needAnimo = true)
        {
            var gdata = App.GetGameData<BtwGameData>();
            var chip = Instantiate(ChipCfg.ChipPerfab);
            var chipTs = chip.transform;
            chipTs.parent = ts.parent;
            chipTs.localPosition = localPos;
            chipTs.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            chipTs.parent = ts;
            _chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = gdata.AnteRate.IndexOf(gold),
                Depth = _chipdepth
            };
            chip.UpdateView(data);
            chip.gameObject.SetActive(true);
            if (!needAnimo) { return; }
            Vector3 target = GetClipPos(ts);
            SetChipTween(chipTs, target, BetTweenInfo);
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
            var chip02 = chipTs.GetComponent<Chip02>();
            if (chip02 == null) return;
            chip02.DelayTime = tweenInfo.RandomDelay ? _ran.Next(0, 30) * 0.01f : tweenInfo.Delay;
            chip02.BeginAnim();
        }

        public override void ThrowChips(int gold, int p, bool isBanker)
        {
            var startPos = isBanker ? BankerChipCfg.StartPos : ChipCfg.StartPos;
            var len = startPos.Length;
            var golds = GetGoldList(gold).ToArray();
            if (len > 0)
            {
                Facade.Instance<MusicManager>().Play("Bet");
                for (int i = 0; i < golds.Length; i++)
                {
                    var randomVal = _ran.Next(0, 100);
                    var randomIndex = randomVal % len;
                    InstantiateChip(ChipCfg.DeskAreas[p], startPos[randomIndex], golds[i]);
                }
            }
        }

        public override void OnDeskClick(Transform ts)
        {

            if (!_isAllowBet) { return; }
            var curClickTime = DateTime.Now.Ticks;
            if (curClickTime - LastClickTime < 0.14f) { return; }
            LastClickTime = curClickTime;
            int index;
            if (!int.TryParse(ts.name, out index) || index < 0) { return; }
            var gdata = App.GetGameData<BtwGameData>();
            if (CurrentSelectChip == null) { return; }
            var chipData = CurrentSelectChip.GetData<ChipData>();
            if (chipData == null) { return; }
            var gold = chipData.Value;
            if (CouldBet(gold))
            {
                var globadata = App.GetRServer<BtwGameServer>();
                globadata.UserBet(index, gold);
                gdata.ThisCanInGold = gdata.ThisCanInGold - gold;
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
                    Chip02 cs = item.GetComponent<Chip02>();
                    List<EventDelegate> de = new List<EventDelegate> { new EventDelegate(cs, "DestoryChip") };
                    SetChipTween(item.transform, target, ResultTweenInfo, de);
                }
            }
        }
        public override void HideChip()
        {
            BetByCoin.SetChipBtnsState(false);
            Select.SetActive(false);
        }

        public override void ShowChip()
        {
            var gdata = App.GetGameData<BtwGameData>();
            if (gdata.IsBanker)
            {
                HideChip();
                return;
            }
            if (!gdata.BeginBet) return;
            BetByCoin.SetChipBtnsState(true);
            Select.SetActive(true);
        }

        public override void Reset()
        {
            MaxGroupBet = App.GetGameData<BtwGameData>().AnteRate[1];
            _selfMaxBet = MaxGroupBet;
            ShowChip();
        }

        public void OnClickLastBetBtn()
        {
            var mgr = App.GetGameManager<BtwGameManager02>();
            mgr.SendLastGameBet();
        }
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