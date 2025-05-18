using UnityEngine;
using YxFramwork.Common;
using Assets.Scripts.Common.components;
using System.Collections.Generic;
using Random = System.Random;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Sfs2X.Entities.Data;
using System;

namespace Assets.Scripts.Game.brnn.brnn_skin02
{
    public class BetCtrl02 : BetCtrl
    {

        [SerializeField]
        public ChipConfig BankerChipCfg = new ChipConfig();

        /// <summary>
        /// 本局自己最大下注额
        /// </summary>
        private int _selfMaxBet;

        public TweenInfo BetTweenInfo;

        public TweenInfo ResultTweenInfo;

        public Color[] SelectedBgColors;

        protected Pool<Chip> BankerChipPool;

        protected override void OnStart()
        {
            base.OnStart();
            BankerChipPool = new Pool<Chip>(BankerChipCfg.ChipPerfab, 50, 0, transform, false);
        }


        public override void OnBetClick(Chip chip)
        {
            int id = chip.GetData<ChipData>().BgId % SelectedBgColors.Length;
            Select.GetComponentInChildren<UISprite>().color = SelectedBgColors[id];
            base.OnBetClick(chip);
        }

        public override void SelfBet(int p, int gold)
        {
            base.SelfBet(p, gold);
            PlayAmazedSoud(gold, ref _selfMaxBet);
        }
  
        private readonly Random _ran = new Random();

        public override void GroupBet(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;
            var sfsArray = responseData.GetSFSArray("coin");
            var startPos = ChipCfg.StartPos;
            Facade.Instance<MusicManager>().Play("groupbet");
            var gdata = App.GameData;
            var selfSeat = gdata.SelfSeat;
            
            int maxGold = 0;
            
            foreach (ISFSObject item in sfsArray)
            {
                int p = item.GetInt("p");
                int gold = item.GetInt("gold");
                int seat = item.GetInt("seat");
                if (seat == selfSeat)
                    continue;

                if (gold >= maxGold)
                {
                    maxGold = gold;
                }

                if(startPos.Length > 0)
                {
                    InstantiateChip(p, startPos[0], gold);
                }
            }

            PlayAmazedSoud(maxGold,ref MaxGroupBet);
        }
       



        public override void InstantiateChip(int p, Vector3 localPos, int gold,bool isBanker = false)
        {
            UIWidget widget = ChipCfg.DeskAreas[p];
            var chip = isBanker ? BankerChipPool.GetOne() : BetPoolArray[p].GetOne();
            var chipTs = chip.transform;
            chipTs.parent = transform;
            chipTs.localPosition = localPos;

            chipTs.parent = widget.transform;
            chipTs.localScale = Vector3.one * 0.5f;
            Chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = App.GetGameData<BrnnGameData>().AnteRate.IndexOf(gold),
                Depth = Chipdepth
            };
            chip.UpdateView(data);
            chip.gameObject.SetActive(true);

            Vector3 target = GetClipPos(widget);

            SetChipAnim(chipTs, target, BetTweenInfo);
        }

      

        private void SetChipAnim(Transform chipTs, Vector3 to,TweenInfo tweenInfo,List<EventDelegate> actionList = null)
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


        public override void ThrowChips(int gold, int p, bool bankerWin)
        {
            var golds = GetGoldList(gold).ToArray();
            var startPos = bankerWin ? BankerChipCfg.StartPos : ChipCfg.StartPos;
            var len = startPos.Length;

            if (len > 0)
            {
                Facade.Instance<MusicManager>().Play("Bet");
                for (int i = 0; i < golds.Length; i++)
                {
                    var randomVal = _ran.Next(0, 100);
                    var randomIndex = randomVal % len;
                    InstantiateChip(p, startPos[randomIndex], golds[i], !bankerWin);    //庄家赢,闲家扔筹码
                }
            }
        }


        public override Vector3 GetClipPos(UIWidget widget)
        {
            var v = Vector3.zero;
            var w = widget.width / 2;
            var h = widget.height / 2;
            var i2 = R.Next(-w, w);
            var i3 = R.Next(-h, h);
            v.x = i2;
            v.y = i3;
            return v;
        }


        public void MoveAllBet(int areaIndex, bool bankerWin)
        {
            var parent = ChipCfg.DeskAreas[areaIndex].transform;
            int childCount = parent.childCount;
            var startPos = bankerWin ? BankerChipCfg.StartPos : ChipCfg.StartPos;
            int len = startPos.Length;
            if(len > 0)
            {
                var target = startPos[0];
                //Transform item;
                for (int i = 0; i < childCount; i++)
                {
                    Transform item = parent.GetChild(0);
                    item.parent = ChipArea;
                    Chip02 cs = item.GetComponent<Chip02>();
                    List<EventDelegate> de = new List<EventDelegate> { new EventDelegate(cs, "DestoryChip") };
                    SetChipAnim(item.transform, target, ResultTweenInfo, de);
                }
            }
        }

       

        public override void HideChip()
        {
            AllBet.SetChipBtnsState(false);
            Select.SetActive(false);
        }

        public override void ShowChip()
        {
            var gdata = App.GetGameData<BrnnGameData>();
            if (gdata.IsBanker)
            {
                HideChip();
                return;
            }
            if (!gdata.BeginBet) return;
            AllBet.SetChipBtnsState(true);
            Select.SetActive(true);
        }

        public override void Reset()
        {
            MaxGroupBet = App.GetGameData<BrnnGameData>().AnteRate[1];
            _selfMaxBet = MaxGroupBet;
            ShowChip();
        }


        public void OnClickLastBetBtn()
        {
            var mgr = App.GetGameManager<BrnnGameManager02>();
            mgr.SendLastGameBet();
        }
    }


    [Serializable]
    public class TweenInfo
    {
        public AnimationCurve TweenCurve ;
        public float Delay;
        public float Duration;
        public List<EventDelegate> OnFinish;
        
        [Tooltip("是否是随机延迟")]
        public bool RandomDelay;
    }

    public class BetInfo
    {
        public int BetP;
        public int BetGold;
        public int BetSeat;
        public BetInfo(ISFSObject betInfo)
        {
            BetP = betInfo.GetInt("p");
            BetGold = betInfo.GetInt("gold");
            BetSeat = betInfo.GetInt("seat");
        }
    }
   
}